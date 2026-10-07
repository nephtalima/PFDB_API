using PFDB.Logging;
using PFDB.ParsingUtility;
using PFDB.PythonTesting;
using PFDB.WeaponUtilityTesting;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using static PFDB.Parsing.DefaultStatisticParameters;

namespace PFDB;






/// <summary>
/// Defines the testing portion of the PFDB CLI.
/// </summary>
public static class Test
{
    //public static IEnumerable<uint> RequiredNumberOfParameters { get; } = new List<uint> { 2 };

    /// <summary>
    /// Handles the test sub-command and calls the appropriate functions.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static void TestCommand(string[] args)
    {
        
		PFDBLogger.LogArguments(new Dictionary<string, object?>() {
            {nameof(args), args}
        });
        switch (args[1].ToLowerInvariant())
        {
            case "all":
            {
                //relying on the first condition failing to avoid indexoutofrange exception
                if(args.Length > 2 && (args[2].ToLowerInvariant() == "help" ||
                    args[2].ToLowerInvariant() == "--help" ||
                    args[2].ToLowerInvariant() == "-h")){
                        StringBuilder builder = new StringBuilder();
                        builder.Append("\n");
                        builder.Append("SUB-COMMAND OPTIONS:\n");
                        builder.Append("pfdb test all (pythonProgramPath) (imageBasePath) (tessbinPath) (acceptableSpaces) (acceptableCorruptedWordSpaces)\n");
                        builder.Append('\n');
                        builder.Append("pythonProgramPath: Path to the Python executable (either Windows or Linux executable). \n\tDefault is the current working directory.\nIt is recommended to use a Python virtual environment for local builds.\n");
                        builder.Append("imageBasePath: Path to the root of the images. \n\tThis folder must contain folders named as version<versionNumber>. versionNumber can be found with 'pfdb inventory'. Can be either relative or absolute path. \n\tDefault is the current working directory.\n");
                        builder.Append("scriptDirectory: Path to the Python script file (usually named impa.py).\n\tThis must be the folder of the script file.\n\tDefault is the currenty workign directory.\n");
                        builder.Append("tessbinPath: Path to the root of the Tesseract training data (this folder is usually called tessbin). \n\tCan be either relative or absolute path. \n\tDefault is the current working directory.\n");
                        builder.Append($"acceptableSpaces: Maxiumum number of acceptable spaces between words when detecting statistics.\n\tDefault is {AcceptableSpaces} spaces.\n");
                        builder.Append($"acceptableCorruptedWordSpaces: Maxiumum number of acceptable spaces between corrupted words when fixing and detecting statistics.\n\tDefault is {AcceptableCorruptedWordSpaces} spaces.\n");
                        builder.Append($"\nCurrent working directory: {Directory.GetCurrentDirectory()}");
                        Console.WriteLine(builder.ToString());
                        break;
                }

                // add all arguments without the first two arguments
                List<string?> allargs = ComponentTester.ArgumentFiller(args: args, requiredNumberOfArgs: 6);


                //we need 6 arguments:
                // python program path
                string? pythonVirtualEnvironmentPath = allargs[0];
                // image base path
                string? imageBasePath = allargs[1];
                // script directory 
                string? scriptDirectory = allargs[2];
                // tessbin path
                string? tessbinPath = allargs[3];

                int? acceptableSpaces = null; //allargs[3]
                int? acceptableCorruptedWordSpaces = null; //allargs[4]
                if(allargs[3] != null){
                    try{
                        acceptableSpaces = Convert.ToInt32(allargs[3]);
                    }catch(Exception exception){
                        PFDBLogger.LogFatal($"An exception was raised when checking {nameof(acceptableSpaces)}: {exception.Message}");
                    }
                    if(acceptableSpaces < 0){
                        PFDBLogger.LogError("acceptableSpaces cannot be negative. Exiting.");
                        break;
                    } 
                }

                if(allargs[4] != null){
                    try{
                        acceptableCorruptedWordSpaces = Convert.ToInt32(allargs[4]);
                    }catch(Exception exception){
                        PFDBLogger.LogFatal($"An exception was raised when checking {nameof(acceptableCorruptedWordSpaces)}: {exception.Message}");
                    }
                    if(acceptableCorruptedWordSpaces < 0){
                        PFDBLogger.LogError("acceptableCorruptedWordSpaces cannot be negative. Exiting.");
                        break;
                    } 
                }


                PFDBLogger.LogInformation($"pythonProgramPath: {pythonVirtualEnvironmentPath}, imageBasePath: {imageBasePath}, tessbinPath: {tessbinPath}, acceptableSpaces number: {acceptableSpaces}, acceptableCorruptedWordSpaces: {acceptableCorruptedWordSpaces}");


                Test.TestAll(
                    pythonVirtualEnvironmentPath: pythonVirtualEnvironmentPath, 
                    imageBasePath: imageBasePath, 
                    scriptDirectory: scriptDirectory,
                    tessbinPath: tessbinPath,
                    acceptableSpaces: acceptableSpaces,
                    acceptableCorruptedWordSpaces: acceptableCorruptedWordSpaces,
                    stringComparisonMethod: null);
                break;
            }
            case "py":
            case "python":
            {
                
                //relying on the first condition failing to avoid indexoutofrange exception
                if(args.Length > 2 && (args[2].ToLowerInvariant() == "help" ||
                    args[2].ToLowerInvariant() == "--help" ||
                    args[2].ToLowerInvariant() == "-h")){
                        StringBuilder builder = new StringBuilder();
                        builder.Append("\n");
                        builder.Append("SUB-COMMAND OPTIONS:\n");
                        builder.Append("pfdb test py(thon) (pythonProgramPath) (imageBasePath) (tessbinPath)\n");
                        builder.Append('\n');
                        builder.Append("pythonProgramPath: Path to the Python executable (either Windows or Linux executable). \n\tDefault is the current working directory.\nIt is recommended to use a Python virtual environment for local builds.\n");
                        builder.Append("imageBasePath: Path to the root of the images. \n\tThis folder must contain folders named as version<versionNumber>. versionNumber can be found with 'pfdb inventory'. Can be either relative or absolute path. \n\tDefault is the current working directory.\n");
                        builder.Append("scriptDirectory: Path to the Python script file (usually named impa.py).\n\tThis must be the folder of the script file.\n\tDefault is the currenty workign directory.\n");
                        builder.Append("tessbinPath: Path to the root of the Tesseract training data (this folder is usually called tessbin). \n\tCan be either relative or absolute path. \n\tDefault is the current working directory.\n");
                        builder.Append($"\nCurrent working directory: {Directory.GetCurrentDirectory()}");
                        Console.WriteLine(builder.ToString());
                        break;
                }

                List<string?> allargs = ComponentTester.ArgumentFiller(args: args, requiredNumberOfArgs: 4);

                //we need 4 arguments:
                // python program path
                string? pythonVirtualEnvironmentPath = allargs[0];
                // image base path
                string? imageBasePath = allargs[1];
                // script directory 
                string? scriptDirectory = allargs[2];
                // tessbin path
                string? tessbinPath = allargs[3];

                PFDBLogger.LogInformation($"pythonProgramPath: {pythonVirtualEnvironmentPath}, imageBasePath: {imageBasePath}, tessbinPath: {tessbinPath}");


                Test.TestPython(pythonVirtualEnvironmentPath: pythonVirtualEnvironmentPath, imageBasePath: imageBasePath, scriptDirectory: scriptDirectory, tessbinPath: tessbinPath); 
                break;
            }
            case "parse":
            {
                //relying on the first condition failing to avoid indexoutofrange exception
                if(args.Length > 2 && (args[2].ToLowerInvariant() == "help" ||
                    args[2].ToLowerInvariant() == "--help" ||
                    args[2].ToLowerInvariant() == "-h")){
                        StringBuilder builder = new StringBuilder();
                        builder.Append("\n");
                        builder.Append("SUB-COMMAND OPTIONS:\n");
                        builder.Append("pfdb test parse (acceptableSpaces) (acceptableCorruptedWordSpaces)\n");
                        builder.Append('\n');
                        builder.Append($"acceptableSpaces: Maxiumum number of acceptable spaces between words when detecting statistics.\n\tDefault is {AcceptableSpaces} spaces.\n");
                        builder.Append($"acceptableCorruptedWordSpaces: Maxiumum number of acceptable spaces between corrupted words when fixing and detecting statistics.\n\tDefault is {AcceptableCorruptedWordSpaces} spaces.\n");
                        builder.Append($"\nCurrent working directory: {Directory.GetCurrentDirectory()}");
                        Console.WriteLine(builder.ToString());
                        break;
                }
                List<string?> allargs = ComponentTester.ArgumentFiller(args, requiredNumberOfArgs: 2 );
                
                int? acceptableSpaces = null;
                int? acceptableCorruptedWordSpaces = null;


                if(allargs[0] != null){
                    acceptableSpaces = Convert.ToInt32(allargs[0]);
                    if(acceptableSpaces < 0){
                        PFDBLogger.LogError("acceptableSpaces cannot be negative. Exiting.");
                        break;
                    } 
                }

                if(allargs[1] != null){
                    acceptableCorruptedWordSpaces = Convert.ToInt32(allargs[1]);
                    if(acceptableCorruptedWordSpaces < 0){
                        PFDBLogger.LogError("acceptableCorruptedWordSpaces cannot be negative. Exiting.");
                        break;
                    } 
                }
                
                //PFDBLogger.LogInformation($"acceptableSpaces number: {acceptableSpaces}, acceptableCorruptedWordSpaces: {acceptableCorruptedWordSpaces}");
                
                
                Test.TestParse(
                    acceptableSpaces: acceptableSpaces,
                    acceptableCorruptedWordSpaces: acceptableCorruptedWordSpaces,
                    null);
                break;
            }
            case "weapon":
            {
                TestWeaponUtility();
                break;
            }
            default:
            {
                Test.DisplayTestHelp();
                break;
            }
        }
    }

    /// <summary>
    /// Calls the main Python testing function.
    /// </summary>
    /// <param name="pythonVirtualEnvironmentPath">Path to the Python executable.</param>
    /// <param name="imageBasePath">Path to the image root folder.</param>
    /// <param name="tessbinPath">Path to the root of the Tesseract training data (this folder is usually called tessbin).</param>
    public static void TestPython(string? pythonVirtualEnvironmentPath, string? imageBasePath, string? scriptDirectory, string? tessbinPath)
    {

		PFDBLogger.LogArguments(new Dictionary<string, object?>() {
            {nameof(pythonVirtualEnvironmentPath), pythonVirtualEnvironmentPath},
            {nameof(imageBasePath), imageBasePath},
            {nameof(tessbinPath), tessbinPath}
        });
        string currentDir = Directory.GetCurrentDirectory();
        PythonTest.Test(pythonVirtualEnvironmentPath ?? currentDir, imageBasePath ?? currentDir, scriptDirectory ?? currentDir, tessbinPath);
    }

    
    /// <summary>
    /// Calls the main parsing testing function.
    /// </summary>
    /// <param name="acceptableSpaces">Specifies the acceptable number spaces between both words.</param>
    /// <param name="acceptableCorruptedWordSpaces">Specifies the acceptable number spaces that a corrupted word can have.</param>
    /// <param name="stringComparisonMethod">Specifies the StringComparison method to be used.</param>
    public static void TestParse(int? acceptableSpaces, int? acceptableCorruptedWordSpaces, StringComparison? stringComparisonMethod)
    {
        PFDBLogger.LogArguments(new Dictionary<string, object?>() {
            {nameof(acceptableSpaces), acceptableSpaces},
            {nameof(acceptableCorruptedWordSpaces), acceptableCorruptedWordSpaces},
            {nameof(stringComparisonMethod), stringComparisonMethod}
        });
        ParseTesting.Test(
            acceptableSpaces ?? AcceptableSpaces,
            acceptableCorruptedWordSpaces ?? AcceptableCorruptedWordSpaces,
            stringComparisonMethod ?? StringComparisonMethod);
    }


    public static void TestWeaponUtility(){
        PFDBLogger.LogArguments(new Dictionary<string, object?>(){});
        WeaponTest.Test();
    }

    /// <summary>
    /// Calls the all the testing functions (Python and parsing).
    /// </summary>
    /// <param name="pythonVirtualEnvironmentPath">Path to the Python executable.</param>
    /// <param name="imageBasePath">Path to the image root folder.</param>
    /// <param name="tessbinPath">Path to the root of the Tesseract training data (this folder is usually called tessbin).</param>
    /// <param name="acceptableSpaces">Specifies the acceptable number spaces between both words.</param>
    /// <param name="acceptableCorruptedWordSpaces">Specifies the acceptable number spaces that a corrupted word can have.</param>
    /// <param name="stringComparisonMethod">Specifies the StringComparison method to be used.</param>
    public static void TestAll(string? pythonVirtualEnvironmentPath, string? imageBasePath, string? scriptDirectory, string? tessbinPath, int? acceptableSpaces, int? acceptableCorruptedWordSpaces, StringComparison? stringComparisonMethod)
    {
		PFDBLogger.LogArguments(new Dictionary<string, object?>() {
            {nameof(pythonVirtualEnvironmentPath), pythonVirtualEnvironmentPath},
            {nameof(imageBasePath), imageBasePath},
            {nameof(tessbinPath), tessbinPath},
            {nameof(acceptableSpaces), acceptableSpaces},
            {nameof(acceptableCorruptedWordSpaces), acceptableCorruptedWordSpaces}
        });
        string currentDir = Directory.GetCurrentDirectory();

        WeaponTest.Test();
        ParseTesting.Test(
            acceptableSpaces ?? AcceptableSpaces,
            acceptableCorruptedWordSpaces ?? AcceptableCorruptedWordSpaces,
            stringComparisonMethod ?? StringComparisonMethod);
        PythonTest.Test(pythonVirtualEnvironmentPath ?? currentDir, imageBasePath ?? currentDir,  scriptDirectory ?? currentDir, tessbinPath);
    }




    /// <summary>
    /// Displays the help for the test sub-command.
    /// </summary>
    public static void DisplayTestHelp()
    {

		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
        /*
         *  all -> test all
         *  python/py -> test python
         *  parse -> test parse
         *
         */

        StringBuilder builder = new StringBuilder();
        builder.Append("\n");
        builder.Append("COMMAND OPTIONS:\n");
        builder.Append("pfdb test (COMMAND) [ARGUMENTS...]\n");
        builder.Append('\n');
        builder.Append("LIST OF SUB-COMMANDS:\n");
        builder.Append("parse\t\tTests the parsing component.\n");
        builder.Append("py(thon)\tTests the Python execution component.\n");
        builder.Append("weapon\tTests the weapon utility component.\n");
        builder.Append("all\t\tTests all components.\n");
        Console.WriteLine(builder.ToString());


    }


}