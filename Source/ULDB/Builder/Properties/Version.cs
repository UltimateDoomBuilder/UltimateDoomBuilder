// ULDB: the program's version. Branding.VERSION is what the user sees (About, crash reports); the numeric
// parts are the assembly's version. Change both for a release (and packaging/RELEASE_NOTES.md, the metainfo file).

using System.Reflection;

[assembly: AssemblyVersion("0.6.1.0")]
[assembly: AssemblyFileVersion("0.6.1.0")]
[assembly: AssemblyInformationalVersion(ULDB.Branding.VERSION)]
