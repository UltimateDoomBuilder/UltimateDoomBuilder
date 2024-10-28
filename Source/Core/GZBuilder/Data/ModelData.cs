#region ================== Namespaces

using System.Collections.Generic;
using CodeImp.DoomBuilder.GZBuilder.Models;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
	internal sealed class ModelData
	{
		#region ================== Constants

		// Keep sage order of extensions as in GZDoom's r_data\models\models.cpp FindGFXFile function. That doesn't
		// list .dds, but just keep it in here
		public static readonly string[] SUPPORTED_TEXTURE_EXTENSIONS = { ".png", ".jpg", ".tga", ".pcx", ".dds" };

		#endregion

		#region ================== Variables

		private ModelLoadState loadstate;
		private Vector3f scale;
		private Vector3f rotationcenter;
		private Matrix transform;
        private Matrix transformrotation;
		private Matrix transformstretched;

		#endregion

		#region ================== Properties

		internal List<string> ModelNames;
		internal List<string> SkinNames;
		internal List<Dictionary<int, string>> SurfaceSkinNames;
		internal List<string> FrameNames;
		internal List<int> FrameIndices;
		internal string Path; // biwa

		internal GZModel Model;

		internal Vector3f Scale { get { return scale; } }
		internal Vector3f RotationCenter { get { return rotationcenter; } set { rotationcenter = value; } }
		internal Matrix Transform { get { /* return (General.Settings.GZStretchView ? transformstretched : transform); */ return transformstretched; } }
        internal Matrix TransformRotation { get { return transformrotation; } }
		internal bool OverridePalette; // Used for voxel models only 
		internal float AngleOffset; // Used for voxel models only
		internal bool InheritActorPitch;
		internal bool UseActorPitch;
		internal bool UseActorRoll;
		internal bool UseRotationCenter;

		internal bool IsVoxel;

		// Hashing
		private static int hashcounter;
		private readonly int hashcode;

		// Disposing
		private bool isdisposed;

		public ModelLoadState LoadState { get { return loadstate; } internal set { loadstate = value; } }

		#endregion

		#region ================== Constructor / Disposer

		internal ModelData() 
		{
			ModelNames = new List<string>();
			SkinNames = new List<string>();
			SurfaceSkinNames = new List<Dictionary<int, string>>();
			FrameNames = new List<string>();
			FrameIndices = new List<int>();
			Path = string.Empty;
			transform = Matrix.Identity;
			transformstretched = Matrix.Identity;
			hashcode = hashcounter++;
		}

		internal void Dispose() 
		{
			// Not already disposed?
			if(!isdisposed) 
			{
				// Clean up
				if(Model != null)
				{
					foreach(Mesh mesh in Model.Meshes) mesh.Dispose();
					foreach(Texture t in Model.Textures) t.Dispose();
					loadstate = ModelLoadState.None;
				}

				// Done
				isdisposed = true;
			}
		}

		internal void SetTransform(Matrix rotation, Matrix offset, Vector3f scale)
		{
			this.scale = scale;
            transformrotation = rotation * Matrix.Scaling(scale);
			transform = rotation * Matrix.Scaling(scale) * offset;
			transformstretched = Matrix.Scaling(1.0f, 1.0f, General.Map.Data.InvertedVerticalViewStretch) * transform;
		}

		//mxd. This greatly speeds up Dictionary lookups
		public override int GetHashCode()
		{
			return hashcode;
		}

		#endregion
	}
}
