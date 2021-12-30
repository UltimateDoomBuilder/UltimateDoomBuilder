#region ================== Namespaces

using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
	/// <summary>
	/// This class provides the camera in Visual Mode
	/// </summary>
	public class VisualCamera
	{
		#region ================== Constants

		public const double ANGLE_FROM_MOUSE = 0.0001;
		public const double MAX_ANGLEZ_LOW = 91.0 / Angle2D.PIDEG;
		public const double MAX_ANGLEZ_HIGH = (360.0 - 91.0) / Angle2D.PIDEG;
		public const double THING_Z_OFFSET = 41.0;
		
		#endregion

		#region ================== Variables

		// Properties
		private Vector3D position;
		private Vector3D target;
		private Vector3D movemultiplier;
		private double anglexy, anglez;
		private Sector sector;
		private double gravity = 1.0; //mxd
		
		#endregion

		#region ================== Properties

		public Vector3D Position { get { return position; } set { position = value; } }
		public Vector3D Target { get { return target; } }
		public double AngleXY { get { return anglexy; } set { anglexy = value; } }
		public double AngleZ { get { return anglez; } set { anglez = value; } }
		public Sector Sector { get { return sector; } internal set { sector = value; UpdateGravity(); } } //mxd
		public Vector3D MoveMultiplier { get { return movemultiplier; } set { movemultiplier = value; } }
		public double Gravity { get { return gravity; } } //mxd
		
		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public VisualCamera()
		{
			// Initialize
			movemultiplier = new Vector3D(1.0, 1.0, 1.0);
			anglexy = 0.0;
			anglez = Angle2D.PI;
			sector = null;
			
			PositionAtThing();
		}
		
		#endregion

		#region ================== Methods

		// Mouse input
		internal void ProcessMouseInput(Vector2D delta)
		{
			// Change camera angles with the mouse changes
			anglexy -= delta.x * ANGLE_FROM_MOUSE;
			if(General.Settings.InvertYAxis)
				anglez -= delta.y * ANGLE_FROM_MOUSE;
			else
				anglez += delta.y * ANGLE_FROM_MOUSE;

			// Normalize angles
			anglexy = Angle2D.Normalized(anglexy);
			anglez = Angle2D.Normalized(anglez);

			// Limit vertical angle
			if(anglez < MAX_ANGLEZ_LOW) anglez = MAX_ANGLEZ_LOW;
			if(anglez > MAX_ANGLEZ_HIGH) anglez = MAX_ANGLEZ_HIGH;
		}

		// Key input
		internal void ProcessMovement(Vector3D deltavec)
		{
			// Calculate camera direction vectors. Multiply by a biggish number, so the decimal digits
			// become less important. This is necessary because the vector will be converted to float
			// when being passed to the renderer, and loss of decimal precision will cause the camera
			// to become jittery when it's far away from the map origin. Muliplying by 10 seems to be
			// enough for non-jittery forward/backward movement, but that's still jittery when looking
			// straight up/down and then moving around, which doesn't happen at something bigger, like
			// the 100 we're using here
			Vector3D camvec = Vector3D.FromAngleXYZ(anglexy, anglez) * 100.0;

			// Position the camera
			position += deltavec;
			
			// Target the camera
			target = position + camvec;
		}

		// This applies the position and angle from the 3D Camera Thing
		// Returns false when it couldn't find a 3D Camera Thing
		public virtual bool PositionAtThing()
		{
			if(General.Settings.GZSynchCameras) return true; //mxd
			Thing modething = null;

			// Find a 3D Mode thing
			foreach(Thing t in General.Map.Map.Things)
			{
				if(t.Type == General.Map.Config.Start3DModeThingType)
				{
					modething = t;
					break; //mxd
				}
			}

			// Found one?
			if(modething != null)
			{
				modething.DetermineSector();
				double z = modething.Position.z;
				if(modething.Sector != null) z += modething.Sector.FloorHeight;
				
				// Position camera here
				Vector3D wantedposition = new Vector3D(modething.Position.x, modething.Position.y, z + THING_Z_OFFSET);
				Vector3D delta = position - wantedposition;
				if(delta.GetLength() > 1.0f) position = wantedposition;

				// Change angle
				double wantedanglexy = modething.Angle + Angle2D.PI;
				if(anglexy != wantedanglexy)
				{
					anglexy = wantedanglexy;
					anglez = Angle2D.PI;
				}
				return true;
			}

			return false;
		}
		
		// This applies the camera position and angle to the 3D Camera Thing
		// Returns false when it couldn't find a 3D Camera Thing
		public virtual bool ApplyToThing()
		{
			if(General.Settings.GZSynchCameras) return true; //mxd
			Thing modething = null;
			
			// Find a 3D Mode thing
			foreach(Thing t in General.Map.Map.Things)
			{
				if(t.Type == General.Map.Config.Start3DModeThingType)
				{
					modething = t;
					break; //mxd
				}
			}

			// Found one?
			if(modething != null)
			{
				int z = (int)position.z; //mxd
				if(sector != null) z -= sector.FloorHeight;

				// Position the thing to match camera
				modething.Move((int)position.x, (int)position.y, z - THING_Z_OFFSET);
				modething.Rotate(anglexy - Angle2D.PI);
				return true;
			}

			return false;
		}

		//mxd
		private void UpdateGravity() 
		{
			if(!General.Map.UDMF || sector == null) return;
			gravity = sector.Fields.GetValue("gravity", 1.0);
		}
		
		#endregion
	}
}
