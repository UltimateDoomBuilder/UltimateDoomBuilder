using System.Collections.Generic;

namespace CodeImp.DoomBuilder.Config
{
    class RequiredArchiveEntry
    {
        private string reqClass;
        private string reqLump;

        public RequiredArchiveEntry(string reqClass, string reqLump)
        {
            this.reqClass = reqClass;
            this.reqLump = reqLump;
        }

        public string Class { get { return reqClass; } }
        public string Lump { get { return reqLump; } }
    }

    class RequiredArchive
    {
        private string id;
        private string filename;
        private bool excludeFromTesting;
        private List<RequiredArchiveEntry> entries;

        public RequiredArchive(string id, string filename, bool excludeFromTesting, List<RequiredArchiveEntry> entries)
        {
            this.id = id;
            this.filename = filename;
            this.excludeFromTesting = excludeFromTesting;
            this.entries = entries;
        }

        public string ID { get { return id;  } }
        public string FileName { get { return filename; } }
        public bool ExcludeFromTesting { get { return excludeFromTesting; } }
        public IReadOnlyCollection<RequiredArchiveEntry> Entries {  get { return entries; } }
    }
}
