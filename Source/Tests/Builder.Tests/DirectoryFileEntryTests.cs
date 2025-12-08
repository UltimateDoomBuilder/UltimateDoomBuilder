using CodeImp.DoomBuilder.IO;

namespace Builder.Tests
{
	public class DirectoryFileEntryTests
	{
		[Theory]
		// Default style
		[InlineData("MAPINFO",						"", FileTitleStyle.DEFAULT, "mapinfo")]
		[InlineData("MAPINFO.txt",					"", FileTitleStyle.DEFAULT, "mapinfo")]
		[InlineData("MAPINFO.moretext.txt",			"", FileTitleStyle.DEFAULT, "mapinfo.moretext")]
		[InlineData("MAPINFO-old",					"", FileTitleStyle.DEFAULT, "mapinfo-old")]
		[InlineData("MAPINFO-old.txt",				"", FileTitleStyle.DEFAULT, "mapinfo-old")]
		[InlineData("MAPINFO-old.moretext.txt",		"", FileTitleStyle.DEFAULT, "mapinfo-old.moretext")]
		[InlineData("ZMAPINFO",						"", FileTitleStyle.DEFAULT, "zmapinfo")]
		[InlineData("ZMAPINFO.txt",					"", FileTitleStyle.DEFAULT, "zmapinfo")]
		[InlineData("ZMAPINFO.moretext.txt",		"", FileTitleStyle.DEFAULT, "zmapinfo.moretext")]
		[InlineData("ZMAPINFO-old",					"", FileTitleStyle.DEFAULT, "zmapinfo-old")]
		[InlineData("ZMAPINFO-old.txt",				"", FileTitleStyle.DEFAULT, "zmapinfo-old")]
		[InlineData("ZMAPINFO-old.moretext.txt",	"", FileTitleStyle.DEFAULT, "zmapinfo-old.moretext")]
		// ZDoom style
		[InlineData("MAPINFO",						"", FileTitleStyle.ZDOOM, "mapinfo")]
		[InlineData("MAPINFO.txt",					"", FileTitleStyle.ZDOOM, "mapinfo")]
		[InlineData("MAPINFO.moretext.txt",			"", FileTitleStyle.ZDOOM, "mapinfo.")]
		[InlineData("MAPINFO-old",					"", FileTitleStyle.ZDOOM, "mapinfo-")]
		[InlineData("MAPINFO-old.txt",				"", FileTitleStyle.ZDOOM, "mapinfo-")]
		[InlineData("MAPINFO-old.moretext.txt",		"", FileTitleStyle.ZDOOM, "mapinfo-")]
		[InlineData("ZMAPINFO",						"", FileTitleStyle.ZDOOM, "zmapinfo")]
		[InlineData("ZMAPINFO.txt",					"", FileTitleStyle.ZDOOM, "zmapinfo")]
		[InlineData("ZMAPINFO.moretext.txt",		"", FileTitleStyle.ZDOOM, "zmapinfo")]
		[InlineData("ZMAPINFO-old",					"", FileTitleStyle.ZDOOM, "zmapinfo")]
		[InlineData("ZMAPINFO-old.txt",				"", FileTitleStyle.ZDOOM, "zmapinfo")]
		[InlineData("ZMAPINFO-old.moretext.txt",	"", FileTitleStyle.ZDOOM, "zmapinfo")]
		// Eternity Engine style
		[InlineData("MAPINFO",						"", FileTitleStyle.ETERNITYENGINE, "mapinfo")]
		[InlineData("MAPINFO.txt",					"", FileTitleStyle.ETERNITYENGINE, "mapinfo")]
		[InlineData("MAPINFO.moretext.txt",			"", FileTitleStyle.ETERNITYENGINE, "mapinfo")]
		[InlineData("MAPINFO-old",					"", FileTitleStyle.ETERNITYENGINE, "mapinfo-")]
		[InlineData("MAPINFO-old.txt",				"", FileTitleStyle.ETERNITYENGINE, "mapinfo-")]
		[InlineData("MAPINFO-old.moretext.txt",		"", FileTitleStyle.ETERNITYENGINE, "mapinfo-")]
		[InlineData("EMAPINFO",						"", FileTitleStyle.ETERNITYENGINE, "emapinfo")]
		[InlineData("EMAPINFO.txt",					"", FileTitleStyle.ETERNITYENGINE, "emapinfo")]
		[InlineData("EMAPINFO.moretext.txt",		"", FileTitleStyle.ETERNITYENGINE, "emapinfo")]
		[InlineData("EMAPINFO-old",					"", FileTitleStyle.ETERNITYENGINE, "emapinfo")]
		[InlineData("EMAPINFO-old.txt",				"", FileTitleStyle.ETERNITYENGINE, "emapinfo")]
		[InlineData("EMAPINFO-old.moretext.txt",	"", FileTitleStyle.ETERNITYENGINE, "emapinfo")]
		public void DirectoryFileEntry_FileTitelShouldWork(string fileName, string fromPath, FileTitleStyle fileTitleStyle, string expectedTitle)
		{
			var entry = new DirectoryFileEntry(fileName, fromPath, fileTitleStyle);
			Assert.Equal(expectedTitle, entry.filetitle);
		}
	}
}