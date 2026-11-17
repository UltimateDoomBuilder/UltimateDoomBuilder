// ULDB: every user-visible name and link for the fork, in one place. Upstream's internal
// names (namespaces, assembly "Builder", "Doom Builder" config file headers) stay unchanged.

using System.Runtime.CompilerServices;

namespace ULDB
{
	public static class Branding
	{
		public const string NAME = "Ultimate Linux Doom Builder";
		public const string SHORT_NAME = "ULDB";
		public const string TAGLINE = "A map editor for GZDoom and UZDoom";
		public const string APP_ID = "io.github.theoriginalzads.UltimateLinuxDoomBuilder";
		public const string EXECUTABLE = "uldb";

		/// <summary>The release this is (also the assembly's informational version; see Properties/Version.cs).</summary>
		public const string VERSION = "0.6.0-beta";

		/// <summary>The cacodemon on the app icon (Assets/Icons).</summary>
		public const string MASCOT = "Cacky, the ULDB Cacodemon";

		public const string REPOSITORY_URL = "https://github.com/theoriginalzads/UltimateLinuxDoomBuilder";
		public const string ISSUES_URL = REPOSITORY_URL + "/issues";

		public const string UPSTREAM_NAME = "Ultimate Doom Builder";
		public const string UPSTREAM_URL = "https://github.com/UltimateDoomBuilder/UltimateDoomBuilder";

		/// <summary>Shown in the About box (see docs/ULDB-Plan.md, section 6).</summary>
		public const string AI_DISCLOSURE = "Linux port developed with AI assistance (Claude by Anthropic).";
		public const string UNOFFICIAL_NOTICE = "Unofficial fork of " + UPSTREAM_NAME + ". Not affiliated with or endorsed by the UDB team; please report problems to ULDB.";

		// Upstream code titles message boxes with Application.ProductName; make those say ULDB.
		[ModuleInitializer]
		internal static void Apply()
		{
			System.Windows.Forms.Application.ProductNameOverride = NAME;
		}
	}
}
