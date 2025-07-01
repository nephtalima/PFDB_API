using System;
using System.Diagnostics;
using System.IO;
using PFDB.Logging;
using PFDB.PythonExecutionUtility;
using PFDB.WeaponUtility;

namespace PFDB.PythonExecution;


/// <summary>
/// Input (consumed) object for <see cref="PythonExecutor"/>, specified for image cropping. This object is responsible for calling the Python script directly.
/// </summary>
public sealed class PythonCropExecutable : IPythonExecutable
{
	private WeaponIdentification _WID;
	private WeaponType _weaponType;
	private string _fileDirectory;
	private string _filename;
	private string _programDirectory;
	private bool _isDefaultConversion;

	private static bool _isWindows = Directory.Exists("C:/");
	private static bool _isLinux = File.Exists("/boot/vmlinuz-linux");

	/// <inheritdoc/>
	public WeaponIdentification WeaponID { get { return _WID; } }

	/// <inheritdoc/>
	public WeaponType WeaponType { get { return _weaponType; } }

	/// <summary>
	/// Directory where the images for reading reside.
	/// </summary>
	public string FileDirectory { get { return _fileDirectory; } }

	/// <inheritdoc/>
	public string Filename { get { return _filename; } }

	/// <inheritdoc/>
	public string ProgramDirectory { get { return _programDirectory; } }

	/// <inheritdoc/>
	public bool IsDefaultConversion { get { return _isDefaultConversion; } }

	private bool _internalExecution = true;
	private bool _untrustedConstruction = true;

	/// <summary>
	/// Unused constructor, but is necessary for C# to not complain. Use <see cref="Construct(string, string, WeaponIdentification, WeaponType, string, bool)"/> instead.
	/// </summary>
	internal PythonCropExecutable()
	{
		_WID = new WeaponIdentification(new PhantomForcesVersion(8, 0, 0), 0, 0, 0);
		_fileDirectory = string.Empty;
		_filename = string.Empty;
		_programDirectory = string.Empty;
		_weaponType = 0;
	}

	/// <inheritdoc/>
	public IPythonExecutable Construct(string filename, string fileDirectory, WeaponIdentification weaponID, WeaponType weaponType, string programDirectory, bool isDefaultConversion = true)
	{
		if (!programDirectory.EndsWith('\\'))
		{
			programDirectory += '\\';
		}
		if (!fileDirectory.EndsWith('\\'))
		{
			fileDirectory += '\\';
		}
		_fileDirectory = fileDirectory;
		_filename = filename;
		_WID = weaponID;
		_weaponType = weaponType;
		_programDirectory = programDirectory;
		_untrustedConstruction = false;
		_isDefaultConversion = isDefaultConversion;
		return this;
	}

	/// <inheritdoc/>
	public void CheckInput()
	{
		PythonAggregateException aggregateException = new PythonAggregateException();
		_internalExecution = false;
		if ((File.Exists(ProgramDirectory + "impa.exe") == false && _isWindows) || (File.Exists(ProgramDirectory + "impa") == false && _isLinux))
		{
			//this shouldn't be logged, the factory ideally should catch and log it
			aggregateException.exceptions.Add(new FileNotFoundException($"The application file, specified at {ProgramDirectory + "impa" + (_isWindows ? ".exe" : string.Empty)} does not exist.", ProgramDirectory + "impa.exe"));
			//throw new FileNotFoundException($"The application file, specified at {ProgramDirectory + "impa.exe"} does not exist.");
		}
		if (File.Exists(FileDirectory + Filename) == false)
		{
			//this shouldn't be logged, the factory ideally should catch and log it
			aggregateException.exceptions.Add(new FileNotFoundException($"The input file, specified at {FileDirectory + Filename} does not exist.", FileDirectory + Filename));
		}

		if (aggregateException.exceptions.Count == 1)
		{
			throw aggregateException.exceptions[0];
		} else if (aggregateException.exceptions.Count > 1)
		{
			throw aggregateException;
		}
		else
		{
			return;
		}
	}

	/// <inheritdoc/>
	public ProcessStartInfo GetProcessStartInfo()
	{
		ProcessStartInfo pyexecute;
		pyexecute = new ProcessStartInfo(ProgramDirectory + "impa" + (_isWindows ? ".exe" : string.Empty), string.Format("{0} {1} {2} {3}", "-w", FileDirectory + Filename, Convert.ToString(WeaponType), WeaponID.Version.VersionNumber.ToString()));
		pyexecute.RedirectStandardOutput = true;
		pyexecute.UseShellExecute = false;
		return pyexecute;
	}

	/// <summary>
	/// Returns the output string of the Python application crop subroutine.
	/// </summary>
	/// <returns>Output string from Python application.</returns>
	public IOutput ReturnOutput()
	{
		if (_internalExecution || _untrustedConstruction)
		{
			//this shouldn't be logged, the factory ideally should catch and log it
			PFDBLogger.LogWarning($"The methods Construct() and CheckInput() have not been called. Do not try to invoke this method directly. File name was {Filename} and weapon ID was {_WID}");
			return new FailedPythonOutput($"The methods Construct() and CheckInput() have not been called. Do not try to invoke this method directly. File name was {Filename} and weapon ID was {_WID}");
		}
		ProcessStartInfo pyexecute = GetProcessStartInfo();
		if (pyexecute != null)
		{
			using (Process? execute = Process.Start(pyexecute))
			{
				if (execute != null)
				{
					using (StreamReader reader = execute.StandardOutput)
					{
						string result = reader.ReadToEnd();

						string command = "Command used: " + pyexecute.Arguments;
						command = command.Replace(FileDirectory + Filename, "...." + PyUtilityClass.CommonExecutionPath(Directory.GetCurrentDirectory() ?? "null", FileDirectory + Filename).relativeForeignPath);
						int width = Console.WindowWidth;
						string line = string.Empty;
						for (int i = 0; i < width; ++i)
						{
							line += "_";
						}

						PythonOutput finalOutput = new PythonOutput(
							$"Executed from: {"...." + PyUtilityClass.CommonExecutionPath(FileDirectory + Filename, Directory.GetCurrentDirectory() ?? "null").relativeForeignPath}{Environment.NewLine}" +
							$"{command}{Environment.NewLine}Computer Information:{Environment.NewLine}" +
							$"Name: {Environment.MachineName}, Processor Count: {Environment.ProcessorCount}, Page Size: {Environment.SystemPageSize}{Environment.NewLine}" +
							$"Working Set Memory: {Environment.WorkingSet}, .NET Version: {Environment.Version}, Operating System: {Environment.OSVersion}{Environment.NewLine}" +
							$"{line}" +
							$"{Environment.NewLine}{Environment.NewLine}" +
						$"{result}");
						return finalOutput;
					}
				}
			}
		}
		//this shouldn't be logged, the factory ideally should catch and log it
		return new FailedPythonOutput("Failed. File name was {Filename} and weapon ID was {_WID}");
	}
}