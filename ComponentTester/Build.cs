using System.IO;
using System.Collections.Generic;
using PFDB.PythonExecution;
using PFDB.PythonFactory;
using PFDB.PythonFactoryUtility;
using PFDB.Logging;
using PFDB.WeaponUtility;
using PFDB.SQLite;
using static PFDB.WeaponUtility.WeaponUtilityClass;

namespace PFDB;

public static class Build
{
	//public static IEnumerable<uint> RequiredNumberOfParameters { get; } = new List<uint> { 3, 4 };

	public static void BuildVersion(PhantomForcesVersion version, string? pythonVirtualEnvironmentDirectory, string? imageBasePath, string? scriptDirectory, string? tessbinPath)
	{
		if (imageBasePath == null) imageBasePath = Directory.GetCurrentDirectory();
		if (pythonVirtualEnvironmentDirectory == null) pythonVirtualEnvironmentDirectory = Directory.GetCurrentDirectory();

		WeaponTable.InitializeEverything();

		Dictionary<Categories, List<int>> weaponNumbers = Helper.GetWeaponNumbersForSpecificVersion(version);


		//set up paths to specific version directories
		IDictionary<PhantomForcesVersion, string> versionAndPathPairs = new Dictionary<PhantomForcesVersion, string>
		{
			{version, $"{imageBasePath}{slash}version{version.VersionNumber}{slash}"}
		};

		//set up factory
		PythonExecutionFactory<PythonTesseractExecutable> factory = new PythonExecutionFactory<PythonTesseractExecutable>(
			weaponNumbers: new Dictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>>{
				{version, weaponNumbers}
			},
			versionAndPathPairs: versionAndPathPairs,
			pythonVirtualEnvironmentDirectory: pythonVirtualEnvironmentDirectory,
			scriptDirectory: scriptDirectory ?? Directory.GetCurrentDirectory(),
			outputDestination: PythonExecutionUtility.OutputDestination.Console | PythonExecutionUtility.OutputDestination.File,
			tessbinPath: tessbinPath
		);

		//start it
		IPythonExecutionFactoryOutput output = factory.Start();

		//outputs
		PFDBLogger.LogWarning("The following files are missing:");
		foreach (string str in output.MissingFiles)
		{
			PFDBLogger.LogInformation(str);
		}


	}

	public static void BuildAllVersions(string? pythonVirtualEnvironmentDirectory, string? imageBasePath, string? scriptDirectory, string? tessbinPath)
	{
		if (imageBasePath == null) imageBasePath = Directory.GetCurrentDirectory();
		if (pythonVirtualEnvironmentDirectory == null) pythonVirtualEnvironmentDirectory = Directory.GetCurrentDirectory();

		WeaponTable.InitializeEverything();

		IDictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>> weaponNumbersForEveryCategoryOfEveryVersion = new Dictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>>();

		IDictionary<PhantomForcesVersion, string> versionAndPathPairs = new Dictionary<PhantomForcesVersion, string>();

		foreach (PhantomForcesVersion version in WeaponTable.ListOfVersions)
		{
			Dictionary<Categories, List<int>> weaponNumbers = Helper.GetWeaponNumbersForSpecificVersion(version);

			weaponNumbersForEveryCategoryOfEveryVersion.Add(version, weaponNumbers);

			versionAndPathPairs.Add(version, $"{imageBasePath}{slash}version{version.VersionNumber}{slash}");

		}

		PythonExecutionFactory<PythonTesseractExecutable> factory = new PythonExecutionFactory<PythonTesseractExecutable>(
			weaponNumbers: weaponNumbersForEveryCategoryOfEveryVersion,
			versionAndPathPairs: versionAndPathPairs,
			pythonVirtualEnvironmentDirectory: pythonVirtualEnvironmentDirectory,
			scriptDirectory: scriptDirectory ?? Directory.GetCurrentDirectory(),
			outputDestination: PythonExecutionUtility.OutputDestination.Console | PythonExecutionUtility.OutputDestination.File,
			tessbinPath: tessbinPath
		);

		//start it
		IPythonExecutionFactoryOutput output = factory.Start();

		//outputs
		PFDBLogger.LogWarning("The following files are missing:");
		foreach (string str in output.MissingFiles)
		{
			PFDBLogger.LogInformation(str);
		}
		
	}
	
	
	
}
