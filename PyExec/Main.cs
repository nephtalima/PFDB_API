using System;
using System.IO;
using PFDB.Logging;
using PFDB.PythonExecution;
using PFDB.PythonExecutionUtility;
using System.Runtime.CompilerServices;
using PFDB.WeaponUtility;
using PFDB.PythonFactory;
using System.Collections.Generic;
using PFDB.PythonFactoryUtility;
using PFDB.SQLite;
using static PFDB.WeaponUtility.WeaponUtilityClass;

/*
Other tests: (maybe todo)
tessbin path not found

*/


namespace PFDB.PythonTesting;

/// <summary>
/// Defines a class that tests functions within PyExec.
/// </summary>
public static class PythonTest
{


    //static string PythonPath = (Directory.Exists("/usr/bin") ? "/mnt/bulkdata/Programming/PFDB/PFDB_API/ImageParserForAPI/dist" : "D:\\Programming\\PFDB\\PFDB_API\\ImageParserForAPI\\dist");
    //static string Path = (Directory.Exists("/usr/bin") ? "/mnt/bulkdata/Programming/PFDB/PFDB_API/ImageParserForAPI" : "D:\\Programming\\PFDB\\PFDB_API\\ImageParserForAPI");

    /// <summary>
    /// Main entry point.
    /// </summary>
    public static void Main()
    {
        PFDBLogger logger = new PFDBLogger(".pfdblog");

        PythonTesseractExecutable tesseractExecutable = new PythonTesseractExecutable();
        tesseractExecutable.Construct("0_2_testimage.png", Directory.GetCurrentDirectory(), new WeaponIdentification(new PhantomForcesVersion("10.1.0"), Categories.AssaultRifles, 15, 0, "AS-VAL"), WeaponType.Primary, Directory.GetCurrentDirectory());

        tesseractExecutable.CheckInput();
        IOutput output = tesseractExecutable.ReturnOutput();
        
        Console.WriteLine($"{output.OutputString}, {((output is Benchmark b) ? b.StopwatchNormal.ElapsedMilliseconds : "")}");

    }

    /// <summary>
    /// Main testing function.
    /// </summary>
    public static bool Test(string pythonProgramPath, string imageBasePath, string? tessbinPath)
    {
        int score = 0;
        if (WeaponTable.InitializeEverything().success == false) return false;

        PFDBLogger.LogInformation("");
        PFDBLogger.LogInformation($"\u001b[1;36mStarting Python testing. (parameters: pythonProgramPath: {pythonProgramPath}, imageBasePath: {imageBasePath}, tessbinPath: {tessbinPath})\u001b[0;0m");
        PFDBLogger.LogInformation("");
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if (PythonInitExecutableTest()) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        PythonExecutorInitExecutableConsoleTest();
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if (PythonExecutorInitExecutableFileTest()) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if (PythonTesseractExecutableTest(tessbinPath)) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if (PythonExecutionFactoryMockedTest(pythonProgramPath, imageBasePath)) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if (PythonExecutionFactoryEmptyTest(pythonProgramPath, imageBasePath, tessbinPath)) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if (PythonExecutionFactoryTesseractTest(pythonProgramPath, imageBasePath, tessbinPath)) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        bool pass = TestingOutput("All PyExec tests", score >= 6, "6", score.ToString());
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        PFDBLogger.LogInformation("");
        return pass;
    }

    /// <summary>
    /// Tests if <see cref="PythonExecutionFactory{InitPythonExecutable}"/> can find files and properly execute them.
    /// </summary>
    /// <returns>Whether this tests passes.</returns>
    public static bool PythonExecutionFactoryTesseractTest(string pythonProgramPath, string imageBasePath, string? tessbinPath)
    {
        //string path = Path;

        Dictionary<Categories, List<int>> weaponNumbers = new Dictionary<Categories, List<int>>();
        PhantomForcesVersion version1001 = new PhantomForcesVersion("10.0.1");



        weaponNumbers.Add(
            Categories.AssaultRifles,
            new List<int>(){
                0
            }
        );
        int expectedAmount = 0;
        foreach (Categories categories in weaponNumbers.Keys)
        {
            expectedAmount += weaponNumbers[categories].Count;
        }
        IDictionary<PhantomForcesVersion, string> versionAndPathPairs = new Dictionary<PhantomForcesVersion, string>
        {
            { version1001, $"{imageBasePath}/version1001/" }
        };
        PythonExecutionFactory<PythonTesseractExecutable> factory =
            new PythonExecutionFactory<PythonTesseractExecutable>(
                new Dictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>>(){
                    {version1001, weaponNumbers}
                }, versionAndPathPairs, pythonProgramPath, OutputDestination.Console, tessbinPath);
        IPythonExecutionFactoryOutput output = factory.Start();
        Console.WriteLine(output.QueueStatusCounter.SuccessCounter);
        PFDBLogger.LogWarning("The following files are missing:");
		foreach (string str in output.MissingFiles)
		{
			PFDBLogger.LogInformation(str);
		}
        int successes = output.QueueStatusCounter.SuccessCounter;
        return TestingOutput("Python execution factory test (queueing, checking, executing)", successes >= expectedAmount, expectedAmount.ToString(), successes.ToString());
    }


    /// <summary>
    /// Tests if <see cref="PythonExecutionFactory{InitExecutable}"/> can detect if we give it an empty list.
    /// </summary>
    /// <returns>Whether this tests passes.</returns>
    public static bool PythonExecutionFactoryEmptyTest(string pythonProgramPath, string imageBasePath, string? tessbinPath)
    {
        Dictionary<Categories, List<int>> weaponNumbers = new Dictionary<Categories, List<int>>();
        PhantomForcesVersion version1001 = new PhantomForcesVersion("10.0.1");

        IDictionary<PhantomForcesVersion, string> versionAndPathPairs = new Dictionary<PhantomForcesVersion, string>
        {
            { version1001, imageBasePath }
        };

        PythonExecutionFactory<PythonTesseractExecutable> factory =
            new PythonExecutionFactory<PythonTesseractExecutable>(
                new Dictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>>(){
                    {version1001, weaponNumbers}
                }, versionAndPathPairs, pythonProgramPath, OutputDestination.Console, tessbinPath);
        IPythonExecutionFactoryOutput output = factory.Start();
        int fails = output.QueueStatusCounter.FailCounter;
        return TestingOutput("Python execution factory test (queueing, checking, executing)", fails == 1, "1", fails.ToString());
    }

    /// <summary>
    /// Tests if <see cref="PythonExecutionFactory{InitPythonExecutable}"/> can find files and properly execute them.
    /// </summary>
    /// <returns>Whether this tests passes.</returns>
    public static bool PythonExecutionFactoryMockedTest(string pythonProgramPath, string imageBasePath)
    {
        //string path = Path;

        Dictionary<Categories, List<int>> weaponNumbers = new Dictionary<Categories, List<int>>();
        PhantomForcesVersion version1001 = new PhantomForcesVersion("10.0.1");
        weaponNumbers.Add(
            Categories.AssaultRifles,
            new List<int>(){
                0,
                1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25
            }
        );
        weaponNumbers.Add(
            Categories.PersonalDefenseWeapons,
            new List<int>(){
                0,
                1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21
            }
        );
        int expectedAmount = 0;
        foreach (Categories categories in weaponNumbers.Keys)
        {
            expectedAmount += weaponNumbers[categories].Count;
        }
        IDictionary<PhantomForcesVersion, string> versionAndPathPairs = new Dictionary<PhantomForcesVersion, string>
        {
            { version1001, $"{imageBasePath}{WeaponUtilityClass.slash}version1001{WeaponUtilityClass.slash}" }
        };

        PythonExecutionFactory<InitExecutable> factory =
            new PythonExecutionFactory<InitExecutable>(
                new Dictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>>(){
                    {version1001, weaponNumbers}
                }, versionAndPathPairs, pythonProgramPath, OutputDestination.Console, null);
        IPythonExecutionFactoryOutput output = factory.Start();

        PFDBLogger.LogWarning("The following files are missing:");
		foreach (string str in output.MissingFiles)
		{
			PFDBLogger.LogInformation(str);
		}
        //Console.WriteLine(output.QueueStatusCounter.SuccessCounter);
        int successes = output.QueueStatusCounter.SuccessCounter;
        return TestingOutput("Python execution factory test (queueing, checking, executing)", successes >= expectedAmount, expectedAmount.ToString(), successes.ToString());
    }

    /// <summary>
    /// Tests if <see cref="InitExecutable"/> returns expected "init object"  output. 
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool PythonInitExecutableTest()
    {
        IPythonExecutable executable = new InitExecutable();
        bool pass = executable.ReturnOutput().OutputString == "init object";
        return TestingOutput("Init Executable detection test", pass, "True", pass.ToString());
    }

    /// <summary>
    /// Tests if the console output option works. I do not know how to test for past console output, so this test is the responsibility of the person reading the logs.
    /// </summary>
    public static void PythonExecutorInitExecutableConsoleTest()
    {
        IPythonExecutor executor = new PythonExecutor(OutputDestination.Console);
        PFDBLogger.LogInformation("Below this message there should be \"init object\".");
        executor.Execute(null);
    }

    /// <summary>
    /// Tests if log and output folders were created.
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool PythonExecutorInitExecutableFileTest()
    {
        IPythonExecutor executor = new PythonExecutor(OutputDestination.File);
        executor.Execute(null);
        bool outputfolderexists = Directory.Exists(Directory.GetCurrentDirectory() + WeaponUtilityClass.slash + PythonExecutor.OutputFolderName + $"{WeaponUtilityClass.slash}0");
        bool logfolderexists = Directory.Exists(Directory.GetCurrentDirectory() + WeaponUtilityClass.slash + PythonExecutor.LogFolderName + $"{WeaponUtilityClass.slash}0");
        bool outputfilecreated = File.Exists(Directory.GetCurrentDirectory() + WeaponUtilityClass.slash + PythonExecutor.OutputFolderName + $"{WeaponUtilityClass.slash}0{WeaponUtilityClass.slash}.pfdb");
        bool logfilecreated = File.Exists(Directory.GetCurrentDirectory() + WeaponUtilityClass.slash + PythonExecutor.LogFolderName + $"{WeaponUtilityClass.slash}0{WeaponUtilityClass.slash}.pfdblog");
        PFDBLogger.LogInformation($"Did it make an output directory? {outputfolderexists}");
        PFDBLogger.LogInformation($"Did it make a log directory? {logfolderexists}");
        PFDBLogger.LogInformation($"Did it make an output file? {outputfilecreated}");
        PFDBLogger.LogInformation($"Did it make a log file? {logfilecreated}");
        //Console.ReadLine();
        if (logfolderexists) Directory.Delete(Directory.GetCurrentDirectory() + WeaponUtilityClass.slash + PythonExecutor.LogFolderName + WeaponUtilityClass.slash + "0", true);
        if (outputfolderexists) Directory.Delete(Directory.GetCurrentDirectory() + WeaponUtilityClass.slash + PythonExecutor.OutputFolderName + WeaponUtilityClass.slash + "/0", true);
        PFDBLogger.LogInformation("Deleted output and log folders (if they even existed)");
        return TestingOutput("Log and output folders + files creation", logfolderexists && outputfolderexists && logfilecreated && outputfilecreated, "True", (logfolderexists && outputfolderexists).ToString());
    }

    /// <summary>
    /// Tests if <see cref="PythonTesseractExecutable"/> is able to read from an image file, correctly read it, and write the output to a new file.
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool PythonTesseractExecutableTest(string? tessbinPath)
    {
        string fileName = "0_2_testimage.png";

        IPythonExecutor executor = new PythonExecutor(OutputDestination.File);
        PythonTesseractExecutable executable = new PythonTesseractExecutable();
        executable.Construct(fileName, Directory.GetCurrentDirectory(),
            new WeaponUtility.WeaponIdentification(new PhantomForcesVersion("10.1.0"), Categories.AssaultRifles, 15, 0, "AS-VAL"),
            WeaponType.Primary, Directory.GetCurrentDirectory(), tessbinPath
            );
        executor.Load(executable);
        PFDBLogger.LogInformation("Executing, this may take a while...");
        executor.Execute(null);
        PFDBLogger.LogInformation("Done executing.");

        int score = 0;
        bool fileExists = File.Exists($"{Directory.GetCurrentDirectory()}{WeaponUtilityClass.slash}{PythonExecutor.OutputFolderName}{WeaponUtilityClass.slash}1010/{fileName}.pfdb");
        if (TestingOutput($"File {fileName}.pfdb exists", fileExists, "True", fileExists.ToString()))
        {
            score++;
        }

        bool containsPredefinedText = executor.Output.OutputString.Contains("RankInfo");
        if (TestingOutput($"Output contains predefined text (RankInfo)", containsPredefinedText, "True", containsPredefinedText.ToString()))
        {
            score++;
        }

        bool containsOCRText = executor.Output.OutputString.Contains("AS VAL");
        if (TestingOutput($"Output contains text only from OCR (AS VAL)", containsOCRText, "True", containsOCRText.ToString()))
        {
            score++;
        }

        return TestingOutput("All 3 above tests", score >= 3, "3", score.ToString());

    }


    /// <summary>
    /// Standardized way of outputting pass/fail condition for various tests.
    /// </summary>
    /// <param name="testName">Name of the test being performed.</param>
    /// <param name="pass">Whether the test passed or failed.</param>
    /// <param name="expectedOutput">Expected output (in string format).</param>
    /// <param name="actualOutput">Actual output (in string format).</param>
    /// <param name="caller">Leave blank unless you wish to override the original test function name.</param>
    /// <returns>Whether the test passed or failed (equivalent to the value of "pass".)</returns>
    public static bool TestingOutput(string testName, bool pass, string expectedOutput, string actualOutput, [CallerMemberName] string caller = "")
    {
        string originalCaller = caller ?? "";
        if (pass)
        {
            PFDBLogger.LogInformation($"{testName}\u001b[1;32m passed.\u001b[0;0m Expected: {expectedOutput}. Got: {actualOutput}", originalCaller);
            return true;
        }
        else
        {
            PFDBLogger.LogError($"{testName}\u001b[1;31m failed.\u001b[0;0m Expected: {expectedOutput}. Got: {actualOutput}", originalCaller);
            return false;
        }
    }
}

/*
<PreBuildEvent Condition="Exists('C:\')"><!--Windows Build-->
py -m PyInstaller -path=[C:\Users\Aethelhelm\AppData\Local\Programs\Python\Python313\Scripts] --workpath $(SolutionDir)ImageParserForAPI\build --distpath $(SolutionDir)ImageParserForAPI\dist -F $(SolutionDir)ImageParserForAPI\impa.py
mkdir $(SolutionDir)PyExec\bin\$(Configuration)\$(TargetFramework)\tessbin
xcopy $(SolutionDir)ImageParserForAPI\tessbin\ $(SolutionDir)PyExec\bin\$(Configuration)\$(TargetFramework)\tessbin\ /y /f /v /e /h /j
xcopy $(SolutionDir)ImageParserForAPI\dist\impa.exe $(SolutionDir)PyExec\bin\$(Configuration)\$(TargetFramework)\ /y /f /v
</PreBuildEvent>
<PreBuildEvent Condition="Exists('/usr/bin') and Exists('$(SolutionDir)')"><!--Linux Build, solution build-->
$(SolutionDir)/bin/python -m PyInstaller --workpath $(SolutionDir)ImageParserForAPI/build --distpath $(SolutionDir)ImageParserForAPI/dist -F -n impa -c $(SolutionDir)ImageParserForAPI/impa.py
mkdir $(SolutionDir)PyExec/bin/$(Configuration)/$(TargetFramework)/tessbin
cp -vaf $(SolutionDir)ImageParserForAPI/tessbin $(SolutionDir)PyExec/bin/$(Configuration)/$(TargetFramework)
cp -vaf $(SolutionDir)ImageParserForAPI/dist/impa $(SolutionDir)PyExec/bin/$(Configuration)/$(TargetFramework)
</PreBuildEvent>
<PreBuildEvent Condition="Exists('/usr/bin') and !Exists('$(SolutionDir)')"><!--Linux Build, project build-->
../../../../bin/python -m PyInstaller --workpath ../../../../ImageParserForAPI/build --distpath ../../../../ImageParserForAPI/dist -F -n impa -c ../../../../ImageParserForAPI/impa.py
mkdir ../../../../PyExec/bin/$(Configuration)/$(TargetFramework)/tessbin
cp -vaf ../../../../ImageParserForAPI/tessbin ../../../../PyExec/bin/$(Configuration)/$(TargetFramework)
cp -vaf ../../../../ImageParserForAPI/dist/impa ../../../../PyExec/bin/$(Configuration)/$(TargetFramework)
</PreBuildEvent>
*/