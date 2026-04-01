
#region ================== Copyright (c) 2026 Boris Iwanski

/*
 * Copyright (c) 2026 Boris Iwanski 
 *
 * This file is part of Ultimate Doom Builder.
 *
 * Ultimate Doom Builder is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * Ultimate Doom Builder is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * Ultimate Doom Builder. If not, see <https://www.gnu.org/licenses/>. 
 * 
 */

#endregion

#region ================== Namespaces

using CodeImp.DoomBuilder.BlockmapExplorer.Controls;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.BlockmapExplorer
{
	[EditMode(DisplayName = "Blockmap Explorer Mode",
			  SwitchAction = "blockmapexplorermode",    // Action name used to switch to this mode
			  ButtonImage = "blockmap.png", // Image resource name for the button
			  ButtonOrder = int.MinValue + 504, // Position of the button (lower is more to the bottom)
			  ButtonGroup = "000_editing",
			  SupportedMapFormats = new[] { "DoomMapSetIO", "HexenMapSetIO" },
			  UseByDefault = true,
			  Volatile = true)]
	public class BlockmapExplorerMode : ClassicMode
	{
		#region ================== Constants

		private const float LINE_LENGTH_SCALER = 0.001f;

		#endregion

		#region ================== Variables

		// Highlighted items
		private int highlightedBlockRow;
		private int highlightedBlockCol;

		private FlatVertex[] overlayGeometry;
		private FlatVertex[] questionableBlocksOverlayGeometry;
		private BlockmapData blockmapData;
		private bool showSharedBlocks;
		private BlockmapExplorerDocker panel;
		private Docker docker;

		#endregion

		#region ================== Constructor / Disposer

		//mxd
		public BlockmapExplorerMode()
		{
			// Do something
		}

		#endregion

		#region ================== Methods

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private short GetShort(byte[] data, int pos) => (short)(data[pos] | (data[pos + 1] << 8));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ushort GetUShort(byte[] data, int pos) => (ushort)(data[pos] | (data[pos + 1] << 8));

		private bool LoadBlockmapData()
		{
			if (!General.Map.LumpExists("BLOCKMAP"))
			{
				General.ToastManager.ShowToast(ToastMessages.BLOCKMAPEXPLORER, ToastType.ERROR, "Failed to engage Blockmap Explorer Mode", "Map has no BLOCKMAP lump.");
				General.Editing.CancelMode();
				return false;
			}

			MemoryStream ms = General.Map.GetLumpData("BLOCKMAP");

			if (ms.Length == 0)
			{
				General.ToastManager.ShowToast(ToastMessages.BLOCKMAPEXPLORER, ToastType.ERROR, "Failed to engage Blockmap Explorer Mode", "BLOCKMAP lump is empty.");
				General.Editing.CancelMode();
				return false;
			}

			byte[] data = ms.ToArray();

			ms.Close();

			// Read the header
			short xOffset = GetShort(data, 0);
			short yOffset = GetShort(data, 2);
			short numCols = GetShort(data, 4);
			short numRows = GetShort(data, 6);

			blockmapData = new BlockmapData
			{
				Offset = new Vector2D(xOffset, yOffset),
				NumCols = numCols,
				NumRows = numRows,
				Blocks = new ConcurrentDictionary<int, BlockmapBlockList>(),
				BlockPointers = new int[numCols, numRows],
				LumpSize = data.Length
			};

			int[] blockListOffsets = new int[numCols * numRows];

			// Read the block list offsets
			for (int i = 0; i < numCols * numRows; i++)
			{
				// The offset is given in number of shorts, so multiply by 2 to get the actual byte offset
				blockListOffsets[i] = GetUShort(data, 8 + i * 2) * 2;
			}

			bool success = true;

			// Read the block lists. Done in parallel since loading large, overflown blocklists can take a while otherwise
			Parallel.For(0, numCols, col =>
			{
				try
				{
					for (int row = numRows - 1; row >= 0; row--)
					{
						int offset = blockListOffsets[row * numCols + col];

						if (offset > data.Length)
						{
							DebugConsole.WriteLine($"BLOCKMAP lump is malformed, block list offset for block ({col}, {row}) points beyond the end of the lump. Offset is {offset}, but lump is only {data.Length} bytes long.");
							blockmapData.BlockPointers[col, row] = -1;
							continue;
						}

						// It's not a sublist if either
						// - the offset is right at the end of the header + blocklist offsets
						// - the short before the offset is 0xFFFF
						// Also check that the offset is greater than 2 to avoid reading before the start of the lump in case of a malformed lump with an offset of 0 or 1
						bool isSublist = (offset > 2) && !((offset == 8 + 2 * numCols * numRows) || GetUShort(data, offset-2) == 0xFFFF);

						// Arleady add a dummy entry to the dictionary to prevent multiple threads from processing the same blocklist
						if (blockmapData.Blocks.TryAdd(offset, null))
						{
							int pos = offset;

							HashSet<int> uniqueIndices = new HashSet<int>();

							while (true)
							{
								ushort index = GetUShort(data, pos); // (ushort)(data[pos] | (data[pos + 1] << 8));
								pos += 2;

								// 0xFFFF (-1 in signed short) indicates the end of the block list
								if (index == 0xFFFF)
									break;

								uniqueIndices.Add(index);
							}

							blockmapData.Blocks[offset] = new BlockmapBlockList(uniqueIndices, isSublist);
						}

						blockmapData.BlockPointers[col, row] = offset;
					}
				}
				catch (IndexOutOfRangeException)
				{
					success = false;
				}
			});

			if (!success)
			{
				General.ToastManager.ShowToast(ToastMessages.BLOCKMAPEXPLORER, ToastType.ERROR, "Failed to engage Blockmap Explorer Mode", "BLOCKMAP lump is malformed, tried to read beyond the end of the lump. Some block lists may not have been loaded.");
				General.Editing.CancelMode();
				return false;
			}

			return true;
		}

		private void UpdateOverlayGeometry()
		{
			if(highlightedBlockCol == -1 || highlightedBlockRow == -1)
			{
				overlayGeometry = new FlatVertex[0];
				return;
			}

			// Create darkened highlight color to use for the highlighted block and the blocks sharing the same block list (if enabled)
			PixelColor highlightColor = General.Colors.Highlight;
			highlightColor.r /= 2;
			highlightColor.g /= 2;
			highlightColor.b /= 2;
			int color = highlightColor.ToInt();

			// Get the column and row of the highlighted block, and if enabled, all blocks sharing the same block list
			List<(int, int)> blocks = showSharedBlocks
				? blockmapData.GetSharedBlocks(highlightedBlockCol, highlightedBlockRow)
				: new List<(int, int)> { (highlightedBlockCol, highlightedBlockRow) };

			overlayGeometry = new FlatVertex[blocks.Count * 6];

			for (int i=0; i < blocks.Count; i++)
			{
				(int col, int row) = blocks[i];

				overlayGeometry[i*6].x = (float)(blockmapData.Offset.x + col * 128);
				overlayGeometry[i*6].y = (float)(blockmapData.Offset.y + row * 128);
				overlayGeometry[i*6].c = color;

				overlayGeometry[i*6 + 1].x = (float)(blockmapData.Offset.x + (col + 1) * 128);
				overlayGeometry[i*6 + 1].y = (float)(blockmapData.Offset.y + row * 128);
				overlayGeometry[i*6 + 1].c = color;

				overlayGeometry[i*6 + 2].x = (float)(blockmapData.Offset.x + (col + 1) * 128);
				overlayGeometry[i*6 + 2].y = (float)(blockmapData.Offset.y + (row + 1) * 128);
				overlayGeometry[i*6 + 2].c = color;

				overlayGeometry[i*6 + 3].x = (float)(blockmapData.Offset.x + col * 128);
				overlayGeometry[i*6 + 3].y = (float)(blockmapData.Offset.y + row * 128);
				overlayGeometry[i*6 + 3].c = color;

				overlayGeometry[i*6 + 4].x = (float)(blockmapData.Offset.x + (col + 1) * 128);
				overlayGeometry[i*6 + 4].y = (float)(blockmapData.Offset.y + (row + 1) * 128);
				overlayGeometry[i*6 + 4].c = color;

				overlayGeometry[i*6 + 5].x = (float)(blockmapData.Offset.x + col * 128);
				overlayGeometry[i*6 + 5].y = (float)(blockmapData.Offset.y + (row + 1) * 128);
				overlayGeometry[i*6 + 5].c = color;
			}
		}

		private void CreateQuestionableBlockOverlay()
		{
			PixelColor highlightColor = General.Colors.Selection;
			highlightColor.r /= 4;
			highlightColor.g /= 4;
			highlightColor.b /= 4;
			int color = highlightColor.ToInt();

			// If enabled, get all blocks that have questionable offsets
			List<(int, int)> blocks = panel.ShowQuestionableBlocks.Checked
				? blockmapData.GetQuestionableBlocks()
				: new List<(int, int)>();

			questionableBlocksOverlayGeometry = new FlatVertex[blocks.Count * 6];

			for (int i = 0; i < blocks.Count; i++)
			{
				(int col, int row) = blocks[i];

				questionableBlocksOverlayGeometry[i * 6].x = (float)(blockmapData.Offset.x + col * 128);
				questionableBlocksOverlayGeometry[i * 6].y = (float)(blockmapData.Offset.y + row * 128);
				questionableBlocksOverlayGeometry[i * 6].c = color;

				questionableBlocksOverlayGeometry[i * 6 + 1].x = (float)(blockmapData.Offset.x + (col + 1) * 128);
				questionableBlocksOverlayGeometry[i * 6 + 1].y = (float)(blockmapData.Offset.y + row * 128);
				questionableBlocksOverlayGeometry[i * 6 + 1].c = color;

				questionableBlocksOverlayGeometry[i * 6 + 2].x = (float)(blockmapData.Offset.x + (col + 1) * 128);
				questionableBlocksOverlayGeometry[i * 6 + 2].y = (float)(blockmapData.Offset.y + (row + 1) * 128);
				questionableBlocksOverlayGeometry[i * 6 + 2].c = color;

				questionableBlocksOverlayGeometry[i * 6 + 3].x = (float)(blockmapData.Offset.x + col * 128);
				questionableBlocksOverlayGeometry[i * 6 + 3].y = (float)(blockmapData.Offset.y + row * 128);
				questionableBlocksOverlayGeometry[i * 6 + 3].c = color;

				questionableBlocksOverlayGeometry[i * 6 + 4].x = (float)(blockmapData.Offset.x + (col + 1) * 128);
				questionableBlocksOverlayGeometry[i * 6 + 4].y = (float)(blockmapData.Offset.y + (row + 1) * 128);
				questionableBlocksOverlayGeometry[i * 6 + 4].c = color;

				questionableBlocksOverlayGeometry[i * 6 + 5].x = (float)(blockmapData.Offset.x + col * 128);
				questionableBlocksOverlayGeometry[i * 6 + 5].y = (float)(blockmapData.Offset.y + (row + 1) * 128);
				questionableBlocksOverlayGeometry[i * 6 + 5].c = color;
			}
		}

		private void UpdateOptions()
		{
			showSharedBlocks = General.Interface.ShiftState;
			UpdateOverlayGeometry();
			General.Interface.RedrawDisplay();
		}

		#endregion

		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("/gzdb/features/classic_modes/mode_blockmapexplorer.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to this mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			if (General.Map.IsChanged)
			{
				General.ToastManager.ShowToast(ToastMessages.BLOCKMAPEXPLORER, ToastType.INFO, "Blockmap Explorer", "Map was changed, rebuilding nodes with testing settings.");

				// We need to build the nodes!
				if (!General.Map.RebuildNodes(General.Map.ConfigSettings.NodebuilderTest, true))
				{
					General.ToastManager.ShowToast(ToastMessages.BLOCKMAPEXPLORER, ToastType.ERROR, "Failed to engage Blockmap Explorer Mode", "Failed to rebuild the nodes.", "Failed to engage Blockmap Explorer Mode: failed to rebuild the nodes");
					General.Editing.CancelMode();
					return;
				}
			}

			panel = new BlockmapExplorerDocker();
			docker = new Docker("blockmapexplorer", "Blockmap Explorer", panel);
			General.Interface.AddDocker(docker);
			General.Interface.SelectDocker(docker);

			panel.ShowQuestionableBlocks.CheckedChanged += (s, e) =>
			{
				CreateQuestionableBlockOverlay();
				General.Interface.RedrawDisplay();
			};

			CustomPresentation presentation = new CustomPresentation();
			presentation.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1.0f, false));
			presentation.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1.0f, true));
			renderer.SetPresentation(presentation);

			if (LoadBlockmapData())
			{
				CreateQuestionableBlockOverlay();

				panel.SetInfo(
					blockmapData.NumRows * blockmapData.NumCols,
					blockmapData.Blocks.Count,
					blockmapData.GetQuestionableOffsetCount(),
					blockmapData.NumCols,
					blockmapData.NumRows,
					blockmapData.GetLinesNotInBlocksCount(General.Map.Map.Linedefs),
					blockmapData.LumpSize
				);
			}
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			General.Interface.RemoveDocker(docker);

			// Hide highlight info
			General.Interface.HideInfo();
		}

		public override void OnUndoEnd()
		{
			base.OnUndoEnd();

			General.Editing.CancelVolatileMode();
		}

		public override void OnRedoEnd()
		{
			base.OnRedoEnd();

			General.Editing.CancelVolatileMode();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();

			// Render lines
			if (renderer.StartPlotter(true))
			{
				int counter = 0;
				List<int> lines = blockmapData.GetLinesInBlock(highlightedBlockCol, highlightedBlockRow);

				foreach (Linedef ld in General.Map.Map.Linedefs)
				{
					// Draw the line in the normal color when it's the next line in the list of lines in the highlighted block, otherwise draw it darkened.
					// This requires that the lines in the list are sorted (which is done in the BlockmapData constructor)
					if (counter < lines.Count && ld.Index == lines[counter])
					{
						renderer.PlotLine(ld.Start.Position, ld.End.Position, renderer.DetermineLinedefColor(ld), LINE_LENGTH_SCALER);
						counter++;
					}
					else
					{
						PixelColor ldc = renderer.DetermineLinedefColor(ld);
						PixelColor pc = new PixelColor(ldc.a, (byte)(ldc.r / 3), (byte)(ldc.g / 3), (byte)(ldc.b / 3));

						renderer.PlotLine(ld.Start.Position, ld.End.Position, pc, LINE_LENGTH_SCALER);
					}
				}

				renderer.Finish();
			}

			if (renderer.StartOverlay(true))
			{
				// Render questionable blocks overlay
				if (panel.ShowQuestionableBlocks.Checked)
					renderer.RenderGeometry(questionableBlocksOverlayGeometry, null, true);

				// Render the highlighted block and all blocks sharing the same block list (if enabled)
				if (overlayGeometry != null && highlightedBlockCol != -1 && highlightedBlockRow != -1)
					renderer.RenderGeometry(overlayGeometry, null, true);

				// Draw blockmap grid. This has to be done last to ensure it's on top of everything and not partially covered by the highlighted block overlay
				for (int row = 0; row <= blockmapData.NumRows; row++)
					renderer.RenderLine(blockmapData.Offset + new Vector2D(0, row * 128), blockmapData.Offset + new Vector2D(blockmapData.NumCols * 128, row * 128), 0.5f, new PixelColor(255, 32, 32, 32), true);

				for (int col = 0; col <= blockmapData.NumCols; col++)
					renderer.RenderLine(blockmapData.Offset + new Vector2D(col * 128, 0), blockmapData.Offset + new Vector2D(col * 128, blockmapData.NumRows * 128), 0.5f, new PixelColor(255, 32, 32, 32), true);

				renderer.Finish();
			}

			renderer.Present();
		}

		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			int oldRow = highlightedBlockRow;
			int oldCol = highlightedBlockCol;

			(highlightedBlockRow, highlightedBlockCol) = blockmapData.GetColumnAndRowByPosition(mousemappos);

			if(oldCol != highlightedBlockCol || oldRow != highlightedBlockRow)
			{
				UpdateOverlayGeometry();

				if (highlightedBlockCol != -1 && highlightedBlockRow != -1)
					panel.SetBlockInfo(highlightedBlockCol, highlightedBlockRow, blockmapData);
				else
					panel.ClearBlockInfo();

				General.Interface.RedrawDisplay();
			}
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			highlightedBlockRow = highlightedBlockCol = -1;

			panel.ClearBlockInfo();

			General.Interface.RedrawDisplay();
		}

		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			UpdateOptions();
		}

		public override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			UpdateOptions();
		}

		#endregion
	}
}
