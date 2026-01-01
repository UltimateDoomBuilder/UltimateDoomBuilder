using CodeImp.DoomBuilder.Data;
using System.Text;

namespace Builder.Tests
{
	public class CvarInfoParserTests
	{
		public static TextResourceData CreateTextResourceDataFromString(string content)
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

			return new TextResourceData(stream, new DataLocation(DataLocation.RESOURCE_DIRECTORY, String.Empty, false, false, false, []), "none");
		}

		[Theory]
		// Integer
		[InlineData("user int mycvar;", "mycvar", 0)]
		[InlineData("user int mycvar=10;", "mycvar", 10)]
		[InlineData("user int mycvar = 10;", "mycvar", 10)]
		// Float
		[InlineData("user float mycvar;", "mycvar", 0f)]
		[InlineData("user float mycvar=10;", "mycvar", 10f)]
		[InlineData("user float mycvar = 10;", "mycvar", 10f)]
		[InlineData("user float mycvar=10.1;", "mycvar", 10.1f)]
		[InlineData("user float mycvar = 10.1;", "mycvar", 10.1f)]
		// Boolean
		[InlineData("user bool mycvar;", "mycvar", false)]
		[InlineData("user bool mycvar=true;", "mycvar", true)]
		[InlineData("user bool mycvar = true;", "mycvar", true)]
		[InlineData("user bool mycvar=false;", "mycvar", false)]
		[InlineData("user bool mycvar = false;", "mycvar", false)]
		// String
		[InlineData("user string mycvar;", "mycvar", "")]
		[InlineData("""user string mycvar="hello";""", "mycvar", "hello")]
		[InlineData("""user string mycvar = "hello";""", "mycvar", "hello")]
		[InlineData("""user string mycvar="";""", "mycvar", "")]
		[InlineData("""user string mycvar = "";""", "mycvar", "")]
		// Handlerclass
		[InlineData("""user handlerclass("MyCvarHandler") int mycvar;""", "mycvar", 0)]
		[InlineData("""user handlerclass ("MyCvarHandler") int mycvar;""", "mycvar", 0)]
		[InlineData("""user handlerclass ("MyCvarHandler" ) int mycvar;""", "mycvar", 0)]
		[InlineData("""user handlerclass ( "MyCvarHandler") int mycvar;""", "mycvar", 0)]
		[InlineData("""user handlerclass ( "MyCvarHandler" ) int mycvar;""", "mycvar", 0)]
		[InlineData("""user handlerclass(MyCvarHandler) int mycvar;""", "mycvar", 0)]
		[InlineData("""user handlerclass (MyCvarHandler) int mycvar;""", "mycvar", 0)]
		[InlineData("""user handlerclass (MyCvarHandler ) int mycvar;""", "mycvar", 0)]
		[InlineData("""user handlerclass ( MyCvarHandler) int mycvar;""", "mycvar", 0)]
		[InlineData("""user handlerclass ( MyCvarHandler ) int mycvar;""", "mycvar", 0)]
		// Handlerclass first
		[InlineData("""handlerclass("MyCvarHandler") user int mycvar;""", "mycvar", 0)]

		public void CvarInfoParser_ValidInput_ShouldWork(string content, string varName, object expectedValue)
		{
			var tr = CreateTextResourceDataFromString(content);
			var parser = new CodeImp.DoomBuilder.ZDoom.CvarInfoParser();

			bool result = parser.Parse(tr, true);

			// Did parsing succeed without errors?
			Assert.True(result, parser.ErrorDescription);
			Assert.False(parser.HasError, parser.ErrorDescription);

			// Does the CVAR exist at all?
			Assert.Contains(varName, parser.Cvars.AllNames);

			//bool success = parser.Cvars.GetValue("mycvar", out object actualValue);
			//Assert.True(success);

			if (expectedValue is int)
			{
				bool cvarExists = parser.Cvars.Integers.TryGetValue(varName, out int intValue);
				Assert.True(cvarExists, "CVAR exists, but is not an int");
				Assert.Equal(expectedValue, intValue);
			}
			else if (expectedValue is float)
			{
				bool cvarExists = parser.Cvars.Floats.TryGetValue(varName, out float floatValue);
				Assert.True(cvarExists, "CVAR exists, but is not a float");
				Assert.Equal(expectedValue, floatValue);
			}
			else if (expectedValue is bool)
			{
				bool cvarExists = parser.Cvars.Booleans.TryGetValue(varName, out bool boolValue);
				Assert.True(cvarExists, "CVAR exists, but is not a bool");
				Assert.Equal(expectedValue, boolValue);
			}
			else if (expectedValue is string)
			{
				bool cvarExists = parser.Cvars.Strings.TryGetValue(varName, out string? stringValue);
				Assert.True(cvarExists, "CVAR exists, but is not a string");
				Assert.Equal(expectedValue, stringValue);
			}
			else
			{
				Assert.Fail("Test case with unsupported expected value type: " + expectedValue.GetType().FullName);
			}
		}

		[Theory]
		[InlineData("int mycvar user;")]
		[InlineData("int mycvar=10 user;")]
		[InlineData("mycvar=10 int user;")]
		[InlineData("mycvar=10 user int;")]
		[InlineData("user int mycvar=10 nosave;")]
		public void CvarInfoParser_InvalidOrder_ShouldFail(string content)
		{
			var tr = CreateTextResourceDataFromString(content);
			var parser = new CodeImp.DoomBuilder.ZDoom.CvarInfoParser();
			bool result = parser.Parse(tr, true);

			// Parsing should fail
			Assert.False(result);
			Assert.True(parser.HasError);
		}
	}
}
