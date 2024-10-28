using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeImp.DoomBuilder.BuilderModes;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.VisualModes
{
	internal abstract class BaseVisualSlope : VisualSlope, IVisualEventReceiver
	{

		#region ================== Variables

		protected readonly BaseVisualMode mode;
		protected readonly SectorLevel level;
		protected Vector3D pickintersect;
		protected double pickrayu;
		protected readonly bool up;
		protected Plane plane;

		#endregion

		#region ================== Properties

		public SectorLevel Level { get { return level; } }

		#endregion

		#region ================== Constructor

		public BaseVisualSlope(BaseVisualMode mode, SectorLevel level, bool up)
		{
			this.mode = mode;
			this.level = level;
			this.up = up;
		}

		#endregion

		#region ================== Events

		// Select or deselect
		public virtual void OnSelectEnd()
		{
			if (this.selected)
			{
				this.selected = false;
				mode.RemoveSelectedObject(this);
				mode.UsedSlopeHandles.Remove(this);
			}
			else
			{
				if (this.pivot)
				{
					General.Interface.DisplayStatus(Windows.StatusType.Warning, "It is not allowed to mark pivot slope handles as selected.");
					return;
				}

				this.selected = true;
				mode.AddSelectedObject(this);
				mode.UsedSlopeHandles.Add(this);
			}
		}

		public void OnEditEnd()
		{
			// We can only have one pivot handle, so remove it from all first
			foreach (KeyValuePair<Sector, List<VisualSlope>> kvp in mode.AllSlopeHandles)
			{
				foreach (VisualSlope handle in kvp.Value)
				{
					if (handle == mode.HighlightedTarget)
					{
						if (handle.Selected)
							General.Interface.DisplayStatus(Windows.StatusType.Warning, "It is not allowed to mark selected slope handles as pivot slope handles.");
						else
						{
							if (handle.Pivot)
							{
								mode.UsedSlopeHandles.Remove(handle);
								handle.Pivot = false;
							}
							else
							{
								mode.UsedSlopeHandles.Add(handle);
								handle.Pivot = true;
							}
						}
					}
					else
					{
						if(!handle.Selected && !handle.SmartPivot)
							mode.UsedSlopeHandles.Remove(handle);

						handle.Pivot = false;
					}
				}
			}
		}

		public abstract void OnChangeTargetHeight(int amount);

		// Return texture name
		public string GetTextureName() { return ""; }

		// Unused
		public void OnSelectBegin() { }
		public void OnEditBegin() { }
		public void OnChangeTargetBrightness(bool up) { }
		public bool OnChangeTextureOffset(int horizontal, int vertical, bool doSurfaceAngleCorrection) { return true; }
		public void OnSelectTexture() { }
		public void OnCopyTexture() { }
		public void OnPasteTexture() { }
		public void OnCopyTextureOffsets() { }
		public void OnPasteTextureOffsets() { }
		public void OnTextureAlign(bool alignx, bool aligny) { }
		public void OnToggleUpperUnpegged() { }
		public void OnToggleLowerUnpegged() { }
		public void OnProcess(long deltatime) { }
		public void OnTextureFloodfill() { }
		public void OnInsert() { }
		public void OnTextureFit(FitTextureOptions options) { } //mxd
		public void ApplyTexture(string texture) { }
		public void ApplyUpperUnpegged(bool set) { }
		public void ApplyLowerUnpegged(bool set) { }
		public void SelectNeighbours(bool select, bool withSameTexture, bool withSameHeight, bool stopatselected) { } //mxd
		public virtual void OnPaintSelectEnd() { } // biwa
		public void OnChangeScale(int x, int y) { }
		public void OnResetTextureOffset() { }
		public void OnResetLocalTextureOffset() { }
		public void OnCopyProperties() { }
		public void OnPasteProperties(bool usecopysetting) { }
		public void OnDelete() { }
		public void OnPaintSelectBegin() { }
		public void OnMouseMove(MouseEventArgs e) { }

		#endregion
	}
}
