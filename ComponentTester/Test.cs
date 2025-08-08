using PFDB.Parsing;
using PFDB.ParsingUtility;
using PFDB.PythonTesting;
using System.Text;
using static PFDB.Parsing.DefaultStatisticParameters;

namespace PFDB;








public static class Test
{
    //public static IEnumerable<uint> RequiredNumberOfParameters { get; } = new List<uint> { 2 };

    public static void TestPython(string? pythonProgramPath, string? imageBasePath, string? tessbinPath)
    {
        string currentDir = Directory.GetCurrentDirectory();
        PythonTest.Test(pythonProgramPath ?? currentDir, imageBasePath ?? currentDir, tessbinPath);
    }

    public static void TestParse(int? acceptableSpaces, int? acceptableCorruptedWordSpaces, StringComparison? stringComparisonMethod)
    {
        ParseTesting.Test(
            acceptableSpaces ?? AcceptableSpaces,
            acceptableCorruptedWordSpaces ?? AcceptableCorruptedWordSpaces,
            stringComparisonMethod ?? StringComparisonMethod);
    }


    public static void TestAll(string? pythonProgramPath, string? imageBasePath, string? tessbinPath, int? acceptableSpaces, int? acceptableCorruptedWordSpaces, StringComparison? stringComparisonMethod)
    {
        string currentDir = Directory.GetCurrentDirectory();
        ParseTesting.Test(
            acceptableSpaces ?? AcceptableSpaces,
            acceptableCorruptedWordSpaces ?? AcceptableCorruptedWordSpaces,
            stringComparisonMethod ?? StringComparisonMethod);
        PythonTest.Test(pythonProgramPath ?? currentDir, imageBasePath ?? currentDir, tessbinPath);
    }


    public static void DisplayTestHelp()
    {

        /*
         *  all -> test all
         *  python/py -> test python
         *  parse -> test parse
         *
         */

        StringBuilder builder = new StringBuilder();
        builder.Append("PFDB - Phantom Forces Database.\n");
        builder.Append("This tool scans a bulk set of images and dumps the data into text files.\n");
        builder.Append("It also parses the files and makes objects from them. Currently this does not do much, but will change.\n");
        builder.Append('\n');
        builder.Append("This sub-command shows the available options for testing.");
        builder.Append('\n');
        builder.Append("COMMAND OPTIONS:\n");
        builder.Append("pfdb test (COMMAND) [ARGUMENTS...]\n");
        builder.Append('\n');
        builder.Append("LIST OF SUB-COMMANDS:\n");
        builder.Append("parse\t\tTests the parsing component.\n");
        builder.Append("py(thon)\tTests the python execution component.\n");
        builder.Append("all\t\tTests all components.\n");
        Console.WriteLine(builder.ToString());


    }


}