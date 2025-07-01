// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Text;
using System.Threading;
using PFDB.Logging;
using PFDB.PythonExecutionUtility;
using PFDB.PythonFactory;


namespace PFDB.PythonExecution;


/// <summary>
/// Indicates if the object is awaitable (using <see cref="ManualResetEvent"/>).
/// </summary>
internal interface IAwaitable
{
	public ManualResetEvent ManualEvent { get; internal set; }

}


/// <summary>
/// Python Execution Class
/// <para>
/// Note: To compile the python file into a working executable, follow these general steps:
/// <list type="number">
/// <item>Ensure Python 3.12 is downloaded</item>
/// <item>Install 'numpy', 'pytesseract', and 'opencv-python' using pip install</item>
/// <item>Install 'PyInstaller'</item>
/// <item>Navigate to the folder containing the python file (impa.py) and run "py -m PyInstaller -path=[path to scripts file, i.e. C:\Users\(youruser)\AppData\Local\Programs\Python\Python312\Scripts] --onefile impa.py"</item>
/// </list>
/// </para>
/// </summary>
public class PythonExecutor : IPythonExecutor, IAwaitable
{
	
	private static string _outputFolderName = "PFDB_outputs";
	private static string _logFolderName = "PFDB_log";

	/// <summary>
	/// Defines the output folder name, where the file output resides (if <see cref="Destination"/> was set to <see cref="OutputDestination.File"/>).
	/// </summary>
	public static string OutputFolderName { get { return _outputFolderName; } }

	/// <summary>
	/// Defines the folder that contains the logs during execution.
	/// </summary>
	public static string LogFolderName {  get { return _logFolderName; } }

	private ManualResetEvent _manualEvent;
	private OutputDestination _destination;
	private IOutput _output;
	private IPythonExecutable _input;
	private bool _hasExecuted;
	private bool _defaultConversion;
#pragma warning disable CS1574 //complains about not finding PythonFactory.PythonExecutionFactory.Start()
	/// <summary>
	/// Signal state for <see cref="PythonFactory.PythonExecutionFactory.Start()"/>.
	/// </summary>
#pragma warning restore CS1574
	public ManualResetEvent ManualEvent { get { return _manualEvent; }  set { _manualEvent = value; } }

	/// <summary>
	/// Output destination for the enclosed <see cref="IOutput"/> object.
	/// </summary>
	public OutputDestination Destination { get { return _destination; } }

	/// <inheritdoc/>
	public IOutput Output { get { return _output; } }

	/// <inheritdoc/>
	public IPythonExecutable Input { get { return _input; } }

	/// <inheritdoc/>
	public bool HasExecuted { get { return _hasExecuted; } }

	/// <inheritdoc/>
	public bool DefaultConversion { get { return _defaultConversion; } }

	/// <summary>
	/// Default constructor. Populates all fields in this class.
	/// </summary>
	/// <param name="destination">Output destination for the enclosed <see cref="IOutput"/> object.</param>
	public PythonExecutor(OutputDestination destination)
	{
		_input = new InitExecutable(); _output = new TestOutput();
		_destination = destination;
		_output = new TestOutput(); //prevent unassigned reference
		_manualEvent = new ManualResetEvent(false); //signal for PythonExecutionFactory
	}


	/// <inheritdoc/>
	/// <exception cref="ArgumentException"></exception>
	public void Load(IPythonExecutable input)
	{
		//edge case where IPythonExecutable is loaded with a FailedPythonOutput type
		/*if(input is IPythonExecutable)
		{
			//throw new ArgumentException("Input cannot be of FailedPythonOutput type.");
		}*/
		_input = input;
		_defaultConversion = _input.IsDefaultConversion;
	}

	/// <summary>
	/// Loads an input class implementing <see cref="IPythonExecutable"/>.
	/// Note: <see cref="IOutput"/> type specifier in parameter <paramref name="input"/> cannot be of <see cref="FailedPythonOutput"/> type.
	/// </summary>
	/// <param name="input">Input for this enclosing class (<see cref="PythonExecutor"/>). Will execute and return in <see cref="Output"/> as a class implementing <see cref="IOutput"/>.</param>
	/// <returns>The same object for chaining.</returns>
	/// <exception cref="ArgumentException"></exception>
	public IPythonExecutor LoadOut(IPythonExecutable input)
	{
		//edge case where IPythonExecutable is loaded with a FailedPythonOutput type
		/*if (input is IPythonExecutable<FailedPythonOutput>)
		{
			//throw new ArgumentException("Input cannot be of FailedPythonOutput type.");
		}*/
		_input = input;
		_defaultConversion = _input.IsDefaultConversion;
		return this;
	}

	/// <inheritdoc/>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentException"></exception>
	public void Execute(object? bs)
	{
		PFDBLogger.LogInformation($"Executing PythonExecutor with file {_input.Filename} with WeaponID {_input.WeaponID.ID} from version {_input.WeaponID.Version.VersionNumber}");
		try
		{
			_input.CheckInput();
		}catch(PythonAggregateException ex)
		{
			StringBuilder builder = new StringBuilder();
			foreach(SystemException exception in ex.exceptions)
			{
				builder.Append($"Exception: {exception.GetType()} ||| {exception.Message}{Environment.NewLine}");
			}
			_output = new FailedPythonOutput(builder.ToString());
			PFDBLogger.LogError($"An error occured while executing input of type {_input}. The affected input was handling {_input.Filename} with weapon ID {_input.WeaponID.ID}{Environment.NewLine}{_output.OutputString}");
		}catch(Exception ex)
		{
			_output = new FailedPythonOutput(ex.Message);
			PFDBLogger.LogError($"An error occured while executing input of type {_input}. The affected input was handling {_input.Filename} with weapon ID {_input.WeaponID.ID}{Environment.NewLine}{_output.OutputString}");
		}
		
		_output = _input.ReturnOutput();
		_hasExecuted = true;
		//Console.WriteLine((int)Destination | (int)OutputDestination.File);
		if (((int)Destination & (int)OutputDestination.File) == (int)OutputDestination.File)
		{
			//Console.WriteLine(Directory.GetCurrentDirectory() ?? "null folder");

			if (!Directory.Exists($"{Directory.GetCurrentDirectory()}/{_outputFolderName}/"))
			{
				Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}/{_outputFolderName}/");
				PFDBLogger.LogInformation($"Creating directory at {Directory.GetCurrentDirectory()}/{_outputFolderName}/ because it did not exist.");
			}
			if (!Directory.Exists($"{Directory.GetCurrentDirectory()}/{_logFolderName}/"))
			{
				Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}/{_logFolderName}/");
				PFDBLogger.LogInformation($"Creating directory at {Directory.GetCurrentDirectory()}/{_logFolderName}/ because it did not exist.");
			}
			if (!Directory.Exists($"{Directory.GetCurrentDirectory()}/{_outputFolderName}/{_input.WeaponID.Version.VersionNumber}")) {
				Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}/{_outputFolderName}/{_input.WeaponID.Version.VersionNumber}");
				PFDBLogger.LogInformation($"Creating directory at {Directory.GetCurrentDirectory()}/{_outputFolderName}/{_input.WeaponID.Version.VersionNumber} because it did not exist.");
			}
			if (!Directory.Exists($"{Directory.GetCurrentDirectory()}/{_logFolderName}/{_input.WeaponID.Version.VersionNumber}")) {
				Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}/{_logFolderName}/{_input.WeaponID.Version.VersionNumber}");
				PFDBLogger.LogInformation($"Creating directory at {Directory.GetCurrentDirectory()}/{_logFolderName}/{_input.WeaponID.Version.VersionNumber} because it did not exist.");
			}

			File.WriteAllText($"{Directory.GetCurrentDirectory()}/{_outputFolderName}/{_input.WeaponID.Version.VersionNumber}/{_input.Filename}.pfdb", _output.OutputString);
			File.WriteAllText($"{Directory.GetCurrentDirectory()}/{_logFolderName}/{_input.WeaponID.Version.VersionNumber}/{_input.Filename}.pfdblog",
				$"Filename: {_input.Filename} {Environment.NewLine}" +
				$"Program Directory: {_input.ProgramDirectory} {Environment.NewLine}" +
				((_output is Benchmark benchmark) ? $"Elapsed time by DateTime (s): { benchmark.StopwatchDateTime.TotalSeconds}, Elapsed time by Stopwatch (s): { benchmark.StopwatchNormal.ElapsedMilliseconds / (double)1000}{Environment.NewLine}": "") +
				((_input is PythonTesseractExecutable inputpyt) ? ($"PF Version: {inputpyt.WeaponID.Version.VersionNumber} {Environment.NewLine}" +
				$"Weapon Type: {inputpyt.WeaponType} {Environment.NewLine}" +
				$"Command Executed: {inputpyt.CommandExecuted} {Environment.NewLine}" +
				$"FileDirectory {inputpyt.FileDirectory} {Environment.NewLine}") : "")
				);
		}
		if (((int)Destination & (int)OutputDestination.Console) == (int)OutputDestination.Console)
		{
			Console.WriteLine(Output.OutputString);
		}
		ManualEvent.Set();
	}

	///<inheritdoc/>
	public new string ToString()
	{
		return _output.ToString();
	}
}
