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

	/// <summary>
	/// Enum that defines the main operation pathways for the CLI.
	/// </summary>
	public enum Operations
	{
		Help = 0,
		Test = 1,
		Build = 2,
		Inventory = 3,
		ManualProofread = 4,
		Setup = 5


	}

	// displays help
	/// <summary>
	/// Displays the main help message.
	/// </summary>
	public static void DisplayHelp()
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
		//ConsoleColor initial = Console.BackgroundColor;
		//Console.BackgroundColor = ConsoleColor.DarkRed;
		StringBuilder builder = new StringBuilder();
		builder.Append("\n");
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
		builder.Append("build\t\t\n");
		builder.Append("inventory\t\t\n");
		builder.Append("proofread\t\t\n");
		Console.WriteLine(builder.ToString());

		//Console.BackgroundColor = initial;
	}

	/*
	 *	Makes sure that we don't get an indexOutOfBoundsException
	 *	basically we don't know how many parameters that the user is going to fill up when they call the command
	 *	so we have a maximum of n parameters
	 *	and so we need to account for the very first two parameters being the main operation followed by a subcommand
	 *	i.e. /pfdb (operation) (sub-command) [actual parameters we care about]
	 *	then we store those into an array and return that
	 *	the amount of arguments before [actual params] constitute the argsOffset number, which is by default 2
	 */


	/// <summary>
	/// Pads argument list when unused parameters are passed.
	/// </summary>
	/// <param name="args">Command-line arguments.</param>
	/// <param name="requiredNumberOfArgs">The required number of arguments for the <b>function</b>, not the command.</param>
	/// <param name="argsOffset">The number of arguments preceding the arguments of interest.</param>
	/// <returns>A list containing the (potentially null) strings to be passed downstream.</returns>
	public static List<string?> ArgumentFiller(string[] args, int requiredNumberOfArgs, int argsOffset = 2)
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>() {
			{nameof(args), args},
			{nameof(requiredNumberOfArgs), requiredNumberOfArgs},
			{nameof(argsOffset), argsOffset}
		});

		List<string?> allargs = new List<string?>(requiredNumberOfArgs);
		for (int i = argsOffset; i < args.Length; ++i)
		{
			allargs.Add(args[i]);
		}

		while (allargs.Count < requiredNumberOfArgs)
		{
			allargs.Add(null);
		}

		//platio sam puno minute za ovo malo sranje...


		return allargs;
	}

	/// <summary>
	/// Decides which operation to use based off of the command line arguments supplied.
	/// </summary
	/// <param name="commandParameterString">Command parameter string.</param>
	/// <returns>Returns the appropriate operation based on the command parameter string.</returns>
	public static Operations OperationDecider(string commandParameterString)
	{

		PFDBLogger.LogArguments(new Dictionary<string, object?>() {
			{nameof(commandParameterString), commandParameterString}
		});
		Operations operation = Operations.Help;
		switch (commandParameterString.ToLowerInvariant())
		{
			case "--help":
			case "help":
				{
					operation = Operations.Help;
					break;
				}
			case "test":
				{
					//if (args.Length < 1 || args.Length > 9) operation = Operations.Help;
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
		return operation;
	}


	/// <summary>
	/// Entry point of the program.
	/// </summary>
	/// <param name="args">Arguments to pass in</param>
	public static void Main(string[] args)
	{
		
		/*
		1. 
		
		*/

		if (args.Length == 0)
		{
			DisplayHelp();
			return;
		}


		for (int i = 0; i < args.Length; ++i)
		{
			//Console.WriteLine($"arg{i}: {args[i]}");
		}

		Operations operation = OperationDecider(args[0]);


		//Console.WriteLine(args[0]);
		//Console.WriteLine(args.Length);


		//Console.WriteLine(operation);


		switch (operation)
		{
			case Operations.Help:
				{
					//todo: add functionality to view the help for each command
					DisplayHelp();
					break;
				}
			case Operations.Test:
				{
					if (args.Length < 2)
					{
						Test.DisplayTestHelp();
						break;
					}

					PFDBLogger logger = new PFDBLogger(".pfdblog");
					Test.TestCommand(args);


					break;
				}
			case Operations.Build:
				{
					if (args.Length < 2)
					{
						//display build help
						break;
					}


					PFDBLogger logger = new PFDBLogger(".pfdblog");
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

		//WeaponTable.InitializeEverything();
		Log.Logger.Information("Application end. Logging has finished.");
		return;
	}

	public static void Inventory()
	{

		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
		//list all weapons from database
		//and sees if they exist
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
