using CodeImp.DoomBuilder.Rendering;
using System.Collections.Generic;

namespace CodeImp.DoomBuilder.GZBuilder.Models
{
    internal class ModelLoadResult
    {
        public List<string> Skins = new List<string>();
        public List<Mesh> Meshes = new List<Mesh>();
        public string Errors;
    }
}
