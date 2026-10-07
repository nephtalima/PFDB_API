using System.Diagnostics;
using System.Collections.Generic;
using PFDB.PythonExecutionUtility;
using PFDB.WeaponUtility;
using PFDB.Logging;

namespace PFDB.PythonExecution;


/// <summary>
/// Dummy initialization implementation of <see cref="IOutput"/>.
/// </summary>
internal class TestOutput : IOutput
{
	/// <summary>
	/// Dummy output string.
	/// </summary>
	public string OutputString { get; private set; }

	/// <summary>
	/// Dummy constructor.
	/// </summary>
	public TestOutput()
	{

		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
		this.OutputString = "init object";
	}
	/// <inheritdoc/>
	public new string ToString()
	{

		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
		return this.OutputString;
	}
}

/// <summary>
/// Dummy initialization implementation of <see cref="IPythonExecutable"/>.
/// </summary>
internal class InitExecutable : IPythonExecutable
{
	/// <summary>
	/// Dummy default constructor.
	/// </summary>
	public InitExecutable() {
		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
	}

	/// <summary>
	/// Dummy filename. Set to <c>string.Empty</c> by default;
	/// </summary>
	public string Filename { get; private set; } = string.Empty;

	/// <summary>
	/// Dummy program directory.
	/// </summary>
	public string PythonVirtualEnvironmentDirectory { get; private set; } = string.Empty;

	/// <summary>
	/// 
	/// </summary>
	public string ScriptDirectory {get; private set; } = string.Empty;

	/// <summary>
	/// Dummy version.
	/// </summary>
	public PhantomForcesVersion Version { get; private set; } = new PhantomForcesVersion("8.0.1");

	public WeaponIdentification WeaponID { get; private set; } = new WeaponIdentification(1000000000000000);
	public WeaponType WeaponType { get; private set; } = WeaponType.Primary;

	public bool IsDefaultConversion { get; private set; } = true;

	/// <summary>
	/// Dummy input-checker.
	/// </summary>
	public void CheckInput()
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
		return;
	}

	/// <summary>
	/// Dummy producer.
	/// </summary>
	/// <returns>Blank <see cref="ProcessStartInfo"/>.</returns>
	public ProcessStartInfo GetProcessStartInfo()
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
		return new ProcessStartInfo();
	}

	/// <summary>
	/// Dummy return.
	/// </summary>
	/// <returns>Blank <see cref="TestOutput"/>.</returns>
	public IOutput ReturnOutput()
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
		return new TestOutput();
	}

	/// <summary>
	/// Dummy constructor.
	/// </summary>
	/// <param name="filename">Dummy parameter.</param>
	/// <param name="fileDirectory">Dummy parameter.</param>
	/// <param name="version">Dummy parameter.</param>
	/// <param name="weaponType">Dummy parameter.</param>
	/// <param name="pythonVirtualEnvironmentDirectory">Dummy parameter.</param>
	/// <param name="scriptDirectory">Directory where the Python impa.py script resides.</param>
	/// <returns>The current object for chaining.</returns>
	public IPythonExecutable Construct(string filename, string fileDirectory, PhantomForcesVersion version, WeaponType weaponType, string pythonVirtualEnvironmentDirectory, string scriptDirectory)
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>() {
			{nameof(filename), filename},
			{nameof(fileDirectory), fileDirectory},
			{nameof(version), version.VersionString},
			{nameof(weaponType), weaponType},
			{nameof(scriptDirectory), scriptDirectory},
			{nameof(pythonVirtualEnvironmentDirectory), pythonVirtualEnvironmentDirectory}
		});
		Filename = filename;
		PythonVirtualEnvironmentDirectory = pythonVirtualEnvironmentDirectory;
		Version = version;
		WeaponType = weaponType;
		PythonVirtualEnvironmentDirectory = pythonVirtualEnvironmentDirectory;
		ScriptDirectory = scriptDirectory;
		return this;
	}

	/// <inheritdoc/>
	public IPythonExecutable Construct(string filename, string fileDirectory, WeaponIdentification weaponID, WeaponType weaponType, string pythonVirtualEnvironmentDirectory, string scriptDirectory, bool isDefaultConversion)
	{
		
		PFDBLogger.LogArguments(new Dictionary<string, object?>() {
			{nameof(filename), filename},
			{nameof(fileDirectory), fileDirectory},
			{nameof(weaponID), weaponID.ID},
			{nameof(weaponType), weaponType},
			{nameof(scriptDirectory), scriptDirectory},
			{nameof(pythonVirtualEnvironmentDirectory), pythonVirtualEnvironmentDirectory}
		});
		Filename = filename;
		PythonVirtualEnvironmentDirectory = pythonVirtualEnvironmentDirectory;
		Version = weaponID.Version;
		WeaponType = weaponType;
		PythonVirtualEnvironmentDirectory = pythonVirtualEnvironmentDirectory;
		ScriptDirectory = scriptDirectory;
		IsDefaultConversion = isDefaultConversion;
		return this;
	}
}