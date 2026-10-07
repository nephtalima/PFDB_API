using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using PFDB.Logging;
using PFDB.PythonExecutionUtility;
using PFDB.WeaponUtility;
using static PFDB.WeaponUtility.WeaponUtilityClass;


namespace PFDB.PythonExecution;


/// <summary>
/// Input (consumed) object for <see cref="PythonExecutor"/>, specified for PyTesseract. This object is responsible for calling the Python script directly.
/// </summary>
public class PythonTesseractExecutable : IPythonExecutable
{
	private protected WeaponIdentification _WID;
	private protected WeaponType _weaponType;
	private protected string _fileDirectory;
	private protected string _filename;
	private protected string _pythonVirtaulEnvironmentDirectory;
	private protected string _scriptDirectory;
	private protected string? _tessbinPath;
	private protected string _commandExecuted;
	private protected bool _isDefaultConversion;

	private static bool _isWindows = Directory.Exists("C:/");
	private static bool _isLinux = File.Exists("/boot/vmlinuz-linux");

	private bool _internalExecution = true;
	private bool _untrustedConstruction = true;

	/// <inheritdoc/>
	public string Filename { get { return _filename; } }

	/// <summary>
	/// Directory where the images for reading reside.
	/// </summary>
	public string FileDirectory { get { return _fileDirectory; } }

	/// <inheritdoc/>
	public WeaponType WeaponType { get { return _weaponType; } }

	/// <inheritdoc/>
	public string PythonVirtualEnvironmentDirectory { get { return _pythonVirtaulEnvironmentDirectory; } }

	/// <inheritdoc/>
	public string ScriptDirectory {get {return _scriptDirectory;} }

	/// <summary>
	/// Path to "tessbin" folder. If null, "tessbin" folder is assumed to be in the same working directory.
	/// </summary>
	public string? TessbinPath { get { return _tessbinPath; } }

	/// <summary>
	/// Command executed by this program.
	/// </summary>
	public string CommandExecuted { get { return _commandExecuted; } }


	/// <inheritdoc/>
	public WeaponIdentification WeaponID { get { return _WID; } }

	/// <inheritdoc/>
	public bool IsDefaultConversion { get { return _isDefaultConversion; } }


	/// <summary>
	/// Unused constructor. Use <see cref="Construct(string, string, WeaponIdentification, WeaponType, string, string, bool)"/> or <see cref="Construct(string, string, WeaponIdentification, WeaponType, string, string, string?, bool)"/> instead.
	/// </summary>
	public PythonTesseractExecutable()
	{
		
        PFDBLogger.LogArguments(new Dictionary<string, object?>(){});
		PFDBLogger.LogDebug("PythonTesseractExecutable unused constructor called");
		_WID = new WeaponIdentification(new PhantomForcesVersion(8, 0, 0), 0, 0, 0);
		_fileDirectory = string.Empty;
		_filename = string.Empty;
		_pythonVirtaulEnvironmentDirectory = string.Empty;
		_scriptDirectory = string.Empty;
		_tessbinPath = null;
		_weaponType = 0;
		_commandExecuted = string.Empty;
	}


	/// <inheritdoc/>
	public IPythonExecutable Construct(string filename, string fileDirectory, WeaponIdentification weaponID, WeaponType weaponType, string pythonVirtualEnvironmentDirectory, string scriptDirectory, bool isDefaultConversion = true)
	{

        PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(filename), filename},
			{nameof(fileDirectory), fileDirectory},
			{nameof(weaponID), weaponID.ID},
			{nameof(weaponType), weaponType},
			{nameof(pythonVirtualEnvironmentDirectory), pythonVirtualEnvironmentDirectory},
			{nameof(scriptDirectory), scriptDirectory},
			{nameof(isDefaultConversion), isDefaultConversion}
		});
		PFDBLogger.LogDebug("PythonTesseractExecutable used constructor called");
		_filename = filename;
		_weaponType = weaponType;
		_isDefaultConversion = isDefaultConversion;
		_tessbinPath = null;
		if (_tessbinPath != null)
		{
			if (_tessbinPath.EndsWith(slash) == false)
			{
				_tessbinPath += slash;
			}
		}
		if (pythonVirtualEnvironmentDirectory.EndsWith(slash) == false)
		{
			pythonVirtualEnvironmentDirectory += slash;
		}
		if (fileDirectory.EndsWith(slash) == false)
		{
			fileDirectory += slash;
		}
		_fileDirectory = fileDirectory;
		_WID = weaponID;
		_pythonVirtaulEnvironmentDirectory = pythonVirtualEnvironmentDirectory;
		_scriptDirectory = scriptDirectory;
		_commandExecuted = string.Empty;
		_untrustedConstruction = false;
		return this;
	}

	/// <summary>
	/// Default constructor.
	/// </summary>
	/// <param name="filename">Name of the file to be read by the Python application.</param>
	/// <param name="fileDirectory">Directory where the images for reading reside.</param>
	/// <param name="weaponType">WeaponType of the weapon, telling the Python application where to read.</param>
	/// <param name="weaponID">Phantom Forces weapon identification.</param>
	/// <param name="pythonVirtualEnvironmentDirectory">Directory where the Python executable resides (Supports virtual environments).</param>
	/// <param name="scriptDirectory">Directory where the Python impa.py script resides.</param>
	/// <param name="tessbinPath">Path to "tessbin" folder. If null, "tessbin" folder is assumed to be in the same working directory.</param>
	/// <param name="isDefaultConversion">Specifies if the images supplied are for default conversion.</param>
	public PythonTesseractExecutable Construct(string filename, string fileDirectory, WeaponIdentification weaponID, WeaponType weaponType, string pythonVirtualEnvironmentDirectory, string scriptDirectory, string? tessbinPath, bool isDefaultConversion = true)
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(filename), filename},
			{nameof(fileDirectory), fileDirectory},
			{nameof(weaponID), weaponID.ID},
			{nameof(weaponType), weaponType},
			{nameof(pythonVirtualEnvironmentDirectory), pythonVirtualEnvironmentDirectory},
			{nameof(scriptDirectory), scriptDirectory},
			{nameof(tessbinPath), tessbinPath},
			{nameof(isDefaultConversion), isDefaultConversion}
		});
		PFDBLogger.LogDebug("PythonTesseractExecutable used constructor called");
		_filename = filename;
		_weaponType = weaponType;
		_tessbinPath = tessbinPath;
		_isDefaultConversion = isDefaultConversion;
		if (_tessbinPath != null)
		{
			if (_tessbinPath.EndsWith(slash) == false)
			{
				_tessbinPath += slash;
			}
		}
		if (pythonVirtualEnvironmentDirectory.EndsWith(slash) == false)
		{
			pythonVirtualEnvironmentDirectory += slash;
		}
		if (fileDirectory.EndsWith(slash) == false)
		{
			fileDirectory += slash;
		}
		_fileDirectory = fileDirectory;
		_WID = weaponID;
		_pythonVirtaulEnvironmentDirectory = pythonVirtualEnvironmentDirectory;
		_scriptDirectory = scriptDirectory;
		_commandExecuted = string.Empty;
		_untrustedConstruction = false;
		return this;
	}

	/// <inheritdoc/>
	public virtual ProcessStartInfo GetProcessStartInfo()
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
		ProcessStartInfo pyexecute;
		StringBuilder command = new StringBuilder("Command used: ");
		if (TessbinPath == null)
		{
			pyexecute = new ProcessStartInfo("./python" , $"{_scriptDirectory}{slash}impa.py -c {FileDirectory + Filename} {Convert.ToString((int)WeaponType)} {WeaponID.Version.VersionNumber.ToString()}");
			command.Append(pyexecute.Arguments);
			command = command.Replace(FileDirectory + Filename, "...." + PyUtilityClass.CommonExecutionPath(Directory.GetCurrentDirectory() ?? "null", FileDirectory + Filename).relativeForeignPath);
		}
		else
		{
			pyexecute = new ProcessStartInfo("./python" , $"{_scriptDirectory}{slash}impa -f {TessbinPath} {FileDirectory + Filename} {Convert.ToString((int)WeaponType)} {WeaponID.Version.VersionNumber.ToString()}");
			command.Append(pyexecute.Arguments);
			command = command.Replace(TessbinPath, "...." + PyUtilityClass.CommonExecutionPath(Directory.GetCurrentDirectory() ?? "null", TessbinPath).relativeForeignPath);
		}
		_commandExecuted = command.ToString();
		pyexecute.WorkingDirectory = PythonVirtualEnvironmentDirectory;
		pyexecute.RedirectStandardOutput = true;
		pyexecute.RedirectStandardError = true;
		pyexecute.UseShellExecute = false;
		PFDBLogger.LogDebug($"Working Directory: {PythonVirtualEnvironmentDirectory}, Arguments: {command.ToString()}");
		return pyexecute;
	}

	/// <inheritdoc/>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="DirectoryNotFoundException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	/// <exception cref="PythonAggregateException"></exception>
	public virtual void CheckInput()
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
		_internalExecution = false;
		PythonAggregateException aggregateException = new PythonAggregateException();
		if ((int)WeaponType > 4 || (int)WeaponType < 1)
		{
			//this shouldn't be logged, the factory ideally should catch and log it
			aggregateException.exceptions.Add(new ArgumentException("weaponType cannot be greater than 3 or less than 0."));
		}
		if (TessbinPath == null)
		{
			if (!Directory.Exists($"{Directory.GetCurrentDirectory()}{slash}tessbin{slash}"))
			{
				//this shouldn't be logged, the factory ideally should catch and log it
				aggregateException.exceptions.Add(new DirectoryNotFoundException($"The tessbin path specified at {Directory.GetCurrentDirectory()}{slash}tessbin{slash} does not exist. Ensure that the directory exists, then try again."));
			}
		}
		else
		{
			if (!Directory.Exists(TessbinPath + $"tessbin{slash}"))
			{
				//this shouldn't be logged, the factory ideally should catch and log it
				aggregateException.exceptions.Add(new DirectoryNotFoundException($"The tessbin path specified at {TessbinPath}tessbin{slash} does not exist. Ensure that the directory exists, then try again."));
			}
		}
		if ((File.Exists(PythonVirtualEnvironmentDirectory + "impa.exe") == false && _isWindows) || (File.Exists(PythonVirtualEnvironmentDirectory + "impa") == false && _isLinux))
		{
			//this shouldn't be logged, the factory ideally should catch and log it
			aggregateException.exceptions.Add(new FileNotFoundException($"The application file, specified at {PythonVirtualEnvironmentDirectory + "impa.exe"} does not exist."));
		}
		if (!File.Exists(FileDirectory + Filename))
		{
			//this shouldn't be logged, the factory ideally should catch and log it
			aggregateException.exceptions.Add(new FileNotFoundException($"The input file, specified at {FileDirectory + Filename} does not exist."));
		}


		if (aggregateException.exceptions.Count == 1)
		{
			throw aggregateException.exceptions[0];
		}
		else if (aggregateException.exceptions.Count > 1)
		{
			throw aggregateException;
		}
		else
		{
			return;
		}
	}

	/// <inheritdoc/>
	public virtual IOutput ReturnOutput()
	{

		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
		if (_internalExecution || _untrustedConstruction)
		{
			//this shouldn't be logged, the factory ideally should catch and log it
			PFDBLogger.LogWarning($"The methods Construct() and CheckInput() have not been called. Do not try to invoke this method directly. File name was {Filename} and weapon ID was {_WID}");
			return new FailedPythonOutput($"The methods Construct() and CheckInputs() have not been called. Do not try to invoke this method directly. File name was {Filename} and weapon ID was {_WID}");
		}

		Benchmark benchmark = new Benchmark();
		benchmark.StartBenchmark();
		string startTime = benchmark.Start.ToString("dddd, MMMM, yyyy HH:mm:ss:fff");

		ProcessStartInfo pyexecute = GetProcessStartInfo();
		using (Process? execute = Process.Start(pyexecute))
		{
			if (execute != null && execute.HasExited != true)
			{

				using (StreamReader reader = execute.StandardError){
					PFDBLogger.LogError($"\u001b[1;31mError occured while executing the Python script:\u001b[0;0m\n {reader.ReadToEnd()}");
				}
				using (StreamReader reader = execute.StandardOutput)
				{
					string result = reader.ReadToEnd();

					string line = string.Empty;
					for (int i = 0; i < Console.WindowWidth; ++i)
					{
						line += "_";
					}

					benchmark.StopBenchmark();
					string endTime = benchmark.End.ToString("dddd, MMMM, yyyy HH:mm:ss:fff");
					benchmark.GetElapsedTimeInSeconds(result);
					return benchmark;
				}
			}
		}
		benchmark.StopBenchmark();

		//this shouldn't be logged, the factory ideally should catch and log it
		return new FailedPythonOutput($"Failed.  File name was {Filename} and weapon ID was {_WID} ");

	}

}