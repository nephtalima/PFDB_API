using PFDB.PythonExecution;
using PFDB.PythonFactory;
using PFDB.WeaponUtility;
using PFDB.Logging;
using Serilog;
using PFDB.PythonExecutionUtility;
using PFDB.SQLite;
using PFDB.ParsingUtility;
using PFDB.PythonTesting;
using PFDB.PythonFactoryUtility;
using static PFDB.Parsing.DefaultStatisticParameters;
using System.Text;

namespace PFDB;

public class ComponentTester
{

	/*
	 *	test -> test capabilities of various components
	 *			(supported: PyExec, FileParse)
	 *			(planned: SQLiteHandler)
	 *			
	 *	build -> builds specific versions
	 *			(supported: existing versions)
	 *			(plan to build all)
	 *	
	 *	inventory -> shows inventory
	 *			(planned: images, text)
	 *	
	 *	
	 *	
	 */

	public enum Operations
	{
		Help = 0,
		Test = 1,
		Build = 2,
		Inventory = 3,
		ManualProofread = 4,
		Setup = 5


	}

	public static void displayHelp()
	{
		//ConsoleColor initial = Console.BackgroundColor;
		//Console.BackgroundColor = ConsoleColor.DarkRed;
		StringBuilder builder = new StringBuilder();

		builder.Append("PFDB - Phantom Forces Database.\n");
		builder.Append("This tool scans a bulk set of images and dumps the data into text files.\n");
		builder.Append("It also parses the files and makes objects from them. Currently this does not do much, but will change.\n");
		builder.Append('\n');
		builder.Append("COMMAND OPTIONS:\n");
		builder.Append("pfdb (COMMAND) [ARGUMENTS...]\n");
		builder.Append('\n');
		builder.Append("LIST OF COMMANDS:\n");
		builder.Append("help\t\tDisplays this help message.\n");
		builder.Append("test\t\tTests Python execution and file parsing capabilities.\n");
		builder.Append("build\n");
		builder.Append("inventory\n");
		builder.Append("proofread\n");
		Console.WriteLine(builder.ToString());

		//Console.BackgroundColor = initial;
	}

	public static void Main(string[] args)
	{

		if (args.Length == 0)
		{
			displayHelp();
			return;
		}


		for (int i = 0; i < args.Length; ++i)
		{
			Console.WriteLine($"arg{i}: {args[i]}");
		}

		Operations operation = Operations.Help;


		Console.WriteLine(args[0]);
		Console.WriteLine(args.Length);


		switch (args[0].ToLowerInvariant())
		{
			case "--help":
			case "help":
				{
					operation = Operations.Help;
					break;
				}
			case "test":
				{
					if (args.Length < 1 || args.Length > 9) operation = Operations.Help;
					operation = Operations.Test;
					break;
				}
			case "build":
				{
					operation = Operations.Build;
					break;
				}
			case "inventory":
				{
					operation = Operations.Inventory;
					break;
				}
			case "proofread":
				{
					operation = Operations.ManualProofread;
					break;
				}
			default:
				{
					operation = Operations.Help;
					break;
				}
		}

		Console.WriteLine(operation);

		PFDBLogger logger = new PFDBLogger(".pfdblog");
		
		const int argsOffset = 2;
		switch (operation)
		{
			case Operations.Help:
				{
					displayHelp();
					break;
				}
			case Operations.Test:
				{
					if (args.Length < 2)
					{
						Test.DisplayTestHelp();
						break;
					}
						
					switch (args[1].ToLowerInvariant())
					{
						case "all":
							{
								string?[]? allargs = { null, null, null, null, null};
								for (int i = argsOffset; i < args.Length; i++)
								{
									//Console.WriteLine($"arg {i}: {args[i] ?? "was null"}");
									allargs[i - argsOffset] = args[i];
								}
	
								int? acceptableSpaces = null;
								int? acceptableCorruptedWordSpaces = null;
								if(allargs[3] != null)
									acceptableSpaces = Convert.ToInt32(allargs[3]);
								if(allargs[4] != null)
									acceptableCorruptedWordSpaces = Convert.ToInt32(allargs[4]);


								PFDBLogger.LogInformation($"pythonProgramPath: {allargs[0]}, imageBasePath: {allargs[1]}, tessbinPath: {allargs[2]}, acceptableSpaces number: {acceptableSpaces}, acceptableCorruptedWordSpaces: {acceptableCorruptedWordSpaces}");
								Test.TestAll(allargs[0], allargs[1], allargs[2],
									acceptableSpaces: acceptableSpaces,
									acceptableCorruptedWordSpaces: acceptableCorruptedWordSpaces,
									null);
								break;
							}
						case "py":
						case "python":
							{
								string?[]? allargs = {null, null, null};
								for (int i = argsOffset; i < args.Length; i++)
								{
									//Console.WriteLine($"arg {i}: {args[i] ?? "was null"}");
									allargs[i - argsOffset] = args[i];
								}


								PFDBLogger.LogInformation($"pythonProgramPath: {allargs[0]}, imageBasePath: {allargs[1]}, tessbinPath: {allargs[2]}");
								Test.TestPython(pythonProgramPath: allargs[0], imageBasePath: allargs[1], tessbinPath: allargs[2]); 
								break;
							}
						case "parse":
							{
								string?[]? allargs = { null, null};
								for (int i = argsOffset; i < args.Length; i++)
								{
									//Console.WriteLine($"arg {i}: {args[i] ?? "was null"}");
									allargs[i - argsOffset] = args[i];
								}
								int? acceptableSpaces = null;
								int? acceptableCorruptedWordSpaces = null;
								if(allargs[0] != null)
									acceptableSpaces = Convert.ToInt32(allargs[0]);
								if(allargs[1] != null)
									acceptableCorruptedWordSpaces = Convert.ToInt32(allargs[1]);

								//PFDBLogger.LogInformation($"acceptableSpaces number: {acceptableSpaces}, acceptableCorruptedWordSpaces: {acceptableCorruptedWordSpaces}");
								Test.TestParse(
									acceptableSpaces: acceptableSpaces,
									acceptableCorruptedWordSpaces: acceptableCorruptedWordSpaces,
									null);
								break;
							}
						default:
							{
								Test.DisplayTestHelp();
								break;
							}
					}
					break;
				}
			case Operations.Build:
				{
					break;
				}
			case Operations.Inventory:
				{
					break;
				}
			case Operations.ManualProofread:
				{
					break;
				}

		}

		/*
		if(test){
			int score = 0;
			if(ParseTesting.Test())score++;

				//pythonProgramPath = args[1]
				//imageBasePath = args[2]

			if (PythonTest.Test(args[1], args[2], null)) score++;
			if(score >= 2){
				Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}");
				PythonTest.TestingOutput("All tests", score >= 2, "2", 2.ToString());
				PFDBLogger.LogInformation("Tests have passed successfully!");
				Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}");
			}
		}

		if(build){
			Console.WriteLine("building");
			//buildAllVersions()
			if(args.Length == 3){
				//full build

				//pythonProgramPath = args[1]
				//imageBasePath = args[2]
				
				buildAllVersions(args[2], args[1], null);
			}else if(args.Length == 4){
				//specific version build 

				//pythonProgramPath = args[1]
				//imageBasePath = args[2]
				
				buildSpecificVersion(args[2], args[1], null, new PhantomForcesVersion(args[3]));
			}else if(args.Length > 4){

			}
			
		}
		*/
		//WeaponTable.InitializeEverything();
		Log.Logger.Information("Application end. Logging has finished.");
		return;
	}

	public static void inventory()
	{
		//list all weapons from database
		//and sees if they exist
	}


	public static void buildAllVersions(string imageBasePath, string pythonProgramPath, string? tessbinPath)
	{
		if (Directory.Exists(imageBasePath) == false)
		{
			PFDBLogger.LogError($"Directory path was not found: {imageBasePath}");
			return;
			//throw new DirectoryNotFoundException($"Directory path was not found: {imageBasePath}");
		}
		IDictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>> list = new Dictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>>();
		IDictionary<PhantomForcesVersion, string> versionAndPathPairs = new Dictionary<PhantomForcesVersion, string>();

		foreach (PhantomForcesVersion version in WeaponTable.ListOfVersions)
		{
			//IDictionary<Categories, int> weaponCounts = WeaponTable.WeaponCounts[version]; //maximum number of weapons in the category
			Dictionary<Categories, List<int>> weaponNumbers = new Dictionary<Categories, List<int>>();
			foreach (Categories category in WeaponTable.WeaponCounts[version].Keys)
			{
				List<int> tempList = new List<int>();
				for (int i = 0; i < WeaponTable.WeaponCounts[version][category]; ++i)
				{
					tempList.Add(i);
				}
				weaponNumbers.Add(category, tempList);
			}
			list.Add(version, weaponNumbers);
			versionAndPathPairs.Add(version, $"{imageBasePath}{PyUtilityClass.slash}version{version.VersionNumber}{PyUtilityClass.slash}");
		}
		PythonExecutionFactory<PythonTesseractExecutable> factory = new PythonExecutionFactory<PythonTesseractExecutable>(list, versionAndPathPairs, pythonProgramPath, OutputDestination.Console | OutputDestination.File, tessbinPath);
		IPythonExecutionFactoryOutput factoryOutput = factory.Start();
		PFDBLogger.LogWarning("The following files are missing:");
		foreach (string str in factoryOutput.MissingFiles)
		{
			Console.WriteLine(str);
		}

	}

	public static void buildSpecificVersion(string imageBasePath, string pythonProgramPath, string? tessbinPath, PhantomForcesVersion version)
	{
		//verify path
		//string sourcePath = "/mnt/bulkdata/Programming/PFDB/PFDB_API/textOutputsByVersion/version1001";
		//PhantomForcesVersion version = new PhantomForcesVersion(10,0,1);


		if (Directory.Exists(imageBasePath) == false)
		{
			PFDBLogger.LogError($"Directory path was not found: {imageBasePath}");
			return;
			//throw new DirectoryNotFoundException($"Directory path was not found: {imageBasePath}");
		}


		//IDictionary<Categories, int> weaponCounts = WeaponTable.WeaponCounts[version]; //maximum number of weapons in the category
		Dictionary<Categories, List<int>> weaponNumbers = new Dictionary<Categories, List<int>>();
		foreach (Categories category in WeaponTable.WeaponCounts[version].Keys)
		{
			List<int> tempList = new List<int>();
			for (int i = 0; i < WeaponTable.WeaponCounts[version][category]; ++i)
			{
				tempList.Add(i);
			}
			weaponNumbers.Add(category, tempList);
		}
		IDictionary<PhantomForcesVersion, string> versionAndPathPairs = new Dictionary<PhantomForcesVersion, string>
		{
			{ version, $"{imageBasePath}{PyUtilityClass.slash}version{version.VersionNumber}{PyUtilityClass.slash}" }
		};
		PythonExecutionFactory<PythonTesseractExecutable> factory =
		new PythonExecutionFactory<PythonTesseractExecutable>(new Dictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>> { { version, weaponNumbers } }, versionAndPathPairs, pythonProgramPath, OutputDestination.Console | OutputDestination.File, tessbinPath);
		IPythonExecutionFactoryOutput factoryOutput = factory.Start();
		PFDBLogger.LogWarning("The following files are missing:");
		foreach (string str in factoryOutput.MissingFiles)
		{
			Console.WriteLine(str);
		}


		//verify number of images

	}
}

/*
< PreBuildEvent Condition="Exists('C:/') and !Exists('./disablePreBuildEvent')"><!--Windows build-->
	xcopy $(SolutionDir)Calculator\Calculator.dll $(SolutionDir)ComponentTester\bin\$(Configuration)\$(TargetFramework)\ /y /f /v		  
	xcopy $(SolutionDir)ImageParserForAPI\dist\impa.exe $(SolutionDir)ImageParserForAPI\ /y /f /v
	mkdir $(SolutionDir)ComponentTester\bin\$(Configuration)\$(TargetFramework)\tessbin
	xcopy $(SolutionDir)ImageParserForAPI\tessbin\ $(SolutionDir)ComponentTester\bin\$(Configuration)\$(TargetFramework)\tessbin\ /y /f /v /e /h /j
	xcopy $(SolutionDir)ImageParserForAPI\dist\impa.exe $(SolutionDir)ComponentTester\bin\$(Configuration)\$(TargetFramework)\ /y /f /v
	xcopy $(SolutionDir)weapon_database.db $(SolutionDir)ComponentTester\bin\$(Configuration)\$(TargetFramework)\ /y /f /v
	</PreBuildEvent>
<PreBuildEvent Condition="Exists('/usr/bin') and Exists('$(SolutionDir)') and !Exists('./disablePreBuildEvent')"><!--Linux build-->
	cp -vaf $(SolutionDir)Calculator/Calculator.dll $(SolutionDir)ComponentTester/bin/$(Configuration)/$(TargetFramework)		  
	cp -vaf $(SolutionDir)ImageParserForAPI/dist/impa $(SolutionDir)ImageParserForAPI
	mkdir $(SolutionDir)ComponentTester/bin/$(Configuration)/$(TargetFramework)/tessbin
	cp -vaf $(SolutionDir)ImageParserForAPI/tessbin $(SolutionDir)ComponentTester/bin/$(Configuration)/$(TargetFramework)/tessbin
	cp -vaf $(SolutionDir)ImageParserForAPI/dist/impa $(SolutionDir)ComponentTester/bin/$(Configuration)/$(TargetFramework)
	cp -vaf $(SolutionDir)weapon_database.db $(SolutionDir)ComponentTester/bin/$(Configuration)/$(TargetFramework)
	</PreBuildEvent>
<PreBuildEvent Condition="Exists('/usr/bin') and !Exists('$(SolutionDir)') and !Exists('./disablePreBuildEvent')"><!--Linux build-->
	cp -vaf ../../../../Calculator/Calculator.dll ../../../../ComponentTester/bin/$(Configuration)/$(TargetFramework)		  
	cp -vaf ../../../../ImageParserForAPI/dist/impa ../../../../ImageParserForAPI
	mkdir ../../../../ComponentTester/bin/$(Configuration)/$(TargetFramework)/tessbin
	cp -vaf ../../../../ImageParserForAPI/tessbin ../../../../ComponentTester/bin/$(Configuration)/$(TargetFramework)/tessbin
	cp -vaf ../../../../ImageParserForAPI/dist/impa ../../../../ComponentTester/bin/$(Configuration)/$(TargetFramework)
	cp -vaf ../../../../weapon_database.db ../../../../ComponentTester/bin/$(Configuration)/$(TargetFramework)
</PreBuildEvent>

*/
