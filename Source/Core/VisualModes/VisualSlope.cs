using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;

namespace CodeImp.DoomBuilder.VisualModes
{
	public enum VisualSlopeType
	{
		Line,
		Vertex
	}

	public abstract class VisualSlope : IVisualPickable
	{
		#region ================== Variables

		// Disposing
		private bool isdisposed;

		// Selected?
		protected bool selected;

		// Pivot?
		protected bool pivot;

		// Smart Pivot?
		protected bool smartpivot;

		// Was changed?
		private bool changed;

		protected VisualSlopeType type;
		protected double length;

		private Matrix position;

		#endregion

		#region ================== Properties

		/// <summary>
		/// Selected or not? This is only used by the core to determine what color to draw it with.
		/// </summary>
		public bool Selected { get { return selected; } set { selected = value; } }

		/// <summary>
		/// Pivot or not? This is only used by the core to determine what color to draw it with.
		/// </summary>
		public bool Pivot { get { return pivot; } set { pivot = value; } }

		/// <summary>
		/// Disposed or not?
		/// </summary>
		public bool IsDisposed { get { return isdisposed; } }

		public bool SmartPivot { get { return smartpivot; } set { smartpivot = value; } }

		public bool Changed { get { return changed; } set { changed = value; } }

		public VisualSlopeType Type { get { return type; } }

		public double Length { get { return length; } }

		public Matrix Position { get { return position; } }

		#endregion

		#region ================== Constructor / Destructor

		public VisualSlope()
		{
			pivot = false;
			smartpivot = false;
		}

		#endregion

		#region ================== Methods

		// This is called before a device is reset (when resized or display adapter was changed)
		public void UnloadResource()
		{
		}

		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public void ReloadResource()
		{
		}

		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should reject
		/// as fast as possible to rule out all geometry that certainly does not touch the line.
		/// </summary>
		public virtual bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			return true;
		}

		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should perform
		/// accurate hit detection and set u_ray to the position on the ray where this hits the geometry.
		/// </summary>
		public virtual bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref double u_ray)
		{
			return true;
		}

		public virtual void Update() {}
		public virtual Vector3D GetPivotPoint() { return new Vector3D(); }
		public virtual VisualSlope GetSmartPivotHandle() { return null; }

		public void SetPosition(Line2D line, Plane plane)
		{
			Line3D line3d = new Line3D(new Vector3D(line.v1, plane.GetZ(line.v1)), new Vector3D(line.v2, plane.GetZ(line.v2)));

			// This vector is perpendicular to the line, with a 90° angle between it and the plane normal
			Vector3D perpendicularvector = Vector3D.CrossProduct(line3d.GetDelta().GetNormal(), plane.Normal) * (-1);

			// This vector is on the plane, with a 90° angle to the perpendicular vector (so effectively
			// it's on the line, but in 3D
			Vector3D linevector = Vector3D.CrossProduct(plane.Normal, perpendicularvector) * (-1);

			Matrix m = Matrix.Null;

			m.M11 = (float)linevector.x;
			m.M12 = (float)linevector.y;
			m.M13 = (float)linevector.z;

			m.M21 = (float)perpendicularvector.x;
			m.M22 = (float)perpendicularvector.y;
			m.M23 = (float)perpendicularvector.z;

			m.M31 = (float)plane.Normal.x;
			m.M32 = (float)plane.Normal.y;
			m.M33 = (float)plane.Normal.z;

			m.M44 = 1.0f;

			// The matrix is at the 0,0 origin, so move it to the start vertex of the line
			Vector3D tp = new Vector3D(line.v1, plane.GetZ(line.v1));

			position = Matrix.Multiply(m, Matrix.Translation(RenderDevice.V3(tp)));
		}

		#endregion
	}
}