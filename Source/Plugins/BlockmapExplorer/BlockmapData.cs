
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

using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using System.Collections.Concurrent;
using System.Collections.Generic;

#endregion

namespace CodeImp.DoomBuilder.BlockmapExplorer
{
	internal class BlockmapData
	{
		public Vector2D Offset;
		public int NumRows;
		public int NumCols;
		public ConcurrentDictionary<int, BlockmapBlockList> Blocks;
		public int[,] BlockPointers;
		public long LumpSize;

		/// <summary>
		/// Returns the column and row of the block that contains the given position, or (-1, -1) if the position is outside the blockmap.
		/// </summary>
		/// <param name="position">Map position to check</param>
		/// <returns>A tuple of the column and row of the block that contains the position, or (-1, -1) if the position is outside the blockmap</returns>
		public (int, int) GetColumnAndRowByPosition(Vector2D position)
		{
			if (position.x < Offset.x || position.y < Offset.y || position.x > Offset.x + NumCols * 128 || position.y > Offset.y + NumRows * 128)
				return (-1, -1);

			int row = (int)(position.y - Offset.y) / 128;
			int col = (int)(position.x - Offset.x) / 128;

			return (row, col);
		}

		/// <summary>
		/// Gets the list of linedef indexes for the block at the given column and row, or an empty list if the column and row are out of bounds or if the block is empty.
		/// </summary>
		/// <param name="col">The column of the block</param>
		/// <param name="row">The row of the block</param>
		/// <returns>A list of linedef indexes for the block, or an empty list if the block is out of bounds or empty</returns>
		public List<int> GetLinesInBlock(int col, int row)
		{
			if (col < 0 || row < 0 || col >= NumCols || row >= NumRows || BlockPointers[col, row] == -1)
				return new List<int>();

			return Blocks[BlockPointers[col, row]].LinedefIndexes;
		}

		/// <summary>
		/// Gets the column and row of all blocks that share the same block list as the block at the given column and row, or an empty list if the column and row are out of bounds or if the block is empty.
		/// </summary>
		/// <param name="col">The column of the block</param>
		/// <param name="row">The row of the block</param>
		/// <returns>A list of tuples containing the column and row of all blocks that share the same block list, or an empty list if the block is out of bounds or empty</returns>
		public List<(int, int)> GetSharedBlocks(int col, int row)
		{
			List<(int, int)> sharedBlocks = new List<(int, int)>();
			
			int offset = BlockPointers[col, row];

			if (offset < 0)
				return sharedBlocks;

			for (int r = 0; r < NumRows; r++)
			{
				for (int c = 0; c < NumCols; c++)
				{
					if (BlockPointers[c, r] == offset)
						sharedBlocks.Add((c, r));
				}
			}

			return sharedBlocks;
		}

		/// <summary>
		/// Gets the column and row of all blocks that have a block list offset that is less than the starting offset of the block lists, which indicates that they may be invalid or corrupted.
		/// </summary>
		/// <returns>A list of tuples containing the column and row of all questionable blocks</returns>
		public List<(int, int)> GetQuestionableBlocks()
		{
			int startBlocklistOffset = 8 + 2 * NumCols * NumRows; // header + block list offsets
			List<(int, int)> questionableBlocks = new List<(int, int)>();

			for (int row = 0; row < NumRows; row++)
			{
				for (int col = 0; col < NumCols; col++)
				{
					if (BlockPointers[col, row] < startBlocklistOffset)
						questionableBlocks.Add((col, row));
				}
			}

			return questionableBlocks;
		}
		
		/// <summary>
		/// Gets the number of block list offsets that are less than the starting offset of the block lists, which indicates that they may be invalid or corrupted.
		/// </summary>
		/// <returns>The number of questionable block list offsets</returns>
		public int GetQuestionableOffsetCount()
		{
			int startBlocklistOffset = 8 + 2 * NumCols * NumRows; // header + block list offsets
			int questionableOffsets = 0;

			foreach (int offset in BlockPointers)
			{
				if (offset < startBlocklistOffset)
					questionableOffsets++;
			}

			return questionableOffsets;
		}

		/// <summary>
		/// Gets the number of linedefs that are not included in any block list.
		/// </summary>
		/// <param name="linedefs">The collection of linedefs to check</param>
		/// <returns>The number of linedefs not included in any block list</returns>
		internal int GetLinesNotInBlocksCount(ICollection<Linedef> linedefs)
		{
			int counter = 0;
			HashSet<int> linesInBlocks = new HashSet<int>();

			foreach (BlockmapBlockList bbl in Blocks.Values)
			{
				if (bbl.LinedefIndexes != null)
					linesInBlocks.UnionWith(bbl.LinedefIndexes);
			}

			foreach (Linedef linedef in linedefs)
			{
				if (!linesInBlocks.Contains(linedef.Index))
					counter++;
			}

			return counter;
		}
	}
}
