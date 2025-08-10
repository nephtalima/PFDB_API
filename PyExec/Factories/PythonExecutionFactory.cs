/*
	God help you if you need to edit this.
	Maybe a refactor in the future...?
*/

using PFDB.PythonExecution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using PFDB.WeaponUtility;
using System.Diagnostics;
using PFDB.Logging;
using PFDB.SQLite;
using PFDB.PythonExecutionUtility;
using PFDB.PythonFactoryUtility;
using System.IO;
using static PFDB.WeaponUtility.WeaponUtilityClass;

namespace PFDB.PythonFactory;


/// <summary>
/// Defines a factory class to execute various classes that inherit from <see cref="IPythonExecutable"/> via <see cref="IPythonExecutor"/>.
/// </summary>
/// <typeparam name="TPythonExecutable">A concrete type that implements <see cref="IPythonExecutable"/>.</typeparam>
public sealed class PythonExecutionFactory<TPythonExecutable> : IPythonExecutionFactory<TPythonExecutable> where TPythonExecutable : IPythonExecutable, new()
{
	/// <summary>
	/// Each IPythonExecutable is contained in its own IPythonExecutor. This queue feeds the machine
	/// </summary>
	private readonly List<IPythonExecutor> _queue;
	private readonly int _coreCount;
	private readonly int _initialQueueCount;
	private bool _isDefaultConversion;
	private IPythonExecutionFactoryOutput? _factoryOutput = null;

	/// <inheritdoc/>
	public IPythonExecutionFactoryOutput? FactoryOutput { get { return _factoryOutput; } }

	/// <inheritdoc/>
	public bool IsDefaultConversion { get { return _isDefaultConversion; } }

	private List<string> missingFiles = new List<string>();


	/// <summary>
	/// Adds to _queue the required objects for the factory to run.
	/// </summary>
	/// <param name="outputDestination"></param>
	/// <param name="categoryGroup"></param>
	/// <param name="weaponNumber"></param>
	/// <param name="imagePath"></param>
	/// <param name="version"></param>
	/// <param name="programDirectory"></param>
	/// <param name="tessbinPath"></param>
	/// <param name="pythonExecutable"></param>
	private void _constructorHelper(OutputDestination outputDestination, Categories categoryGroup, int weaponNumber, string imagePath, PhantomForcesVersion version, string programDirectory, string? tessbinPath, TPythonExecutable pythonExecutable)
	{
		//create a PythonExecutor object 
		PythonExecutor py = new PythonExecutor(outputDestination);
		PFDBLogger.LogDebug("PythonExecutionFactory information. Is current object a PythonTesseractExecutable?", parameter: pythonExecutable is PythonTesseractExecutable);

		//check if we have multiple screenshots for the current version
		for (int index = 0; index < version.MultipleScreenshotsCheck(); index++)
		{
			//add trailing slash
			if (imagePath.EndsWith(slash) == false)
			{
				imagePath += slash;
			}

			string temp = "";

			//change screenshot index based on version
			if (version.IsLegacy)
			{
				temp = $"_{index + 1}";
			}
			else if (version.VersionNumber > 1012)
			{
				temp = $"_{index}";
			}

			//check if the file exists, add to missing files list
			if (File.Exists($"{imagePath}{(int)categoryGroup}_{weaponNumber}{temp}.png") == false)
			{
				PFDBLogger.LogError($"The targeted file ({imagePath}{(int)categoryGroup}_{weaponNumber}{temp}.png) was not found. Skipping.", parameter: imagePath);
				missingFiles.Add($"{imagePath}{(int)categoryGroup}_{weaponNumber}{temp}.png");
				continue; //not found, skip to the next one
			}

			//big boi time
			pythonExecutable = new TPythonExecutable(); // do not remove this line otherwise you will spend hours of your time trying to figure out why it's only building 0_25.png
			if (pythonExecutable is PythonTesseractExecutable pytessexec) //check if we are using pythontesseract (note that other cases should be handled should they need to be added
			{
				try
				{
					/*var weaponID = WeaponTable.WeaponIDCache[version].First(
									x => x.weaponNumber == weaponNumber &&
									(Categories)x.categoryNumber == categoryGroup
									).weaponID;*/
					//load IPythonExecutor with the appropriate IPythonExecutable
					IPythonExecutor pythonExecutor = new PythonExecutor(outputDestination);
					pythonExecutor.Load(pytessexec.Construct(
								$"{(int)categoryGroup}_{weaponNumber}{temp}.png",
								imagePath,
								WeaponTable.WeaponIDAndCategoryWeaponNumbers[version].First(x => x.weaponID.Category == categoryGroup && x.weaponNumber == weaponNumber).weaponID,
								WeaponUtilityClass.GetWeaponType(categoryGroup),
								programDirectory,
								tessbinPath,
								_isDefaultConversion
							));
					_queue.Add(pythonExecutor);
				}
				catch (InvalidOperationException) //when something is missing from the weaponIDCache list
				{
					continue; //skip, it's not found g
				}
			}
			else
			{
				//here we are not using PythonTesseractExeutable, therefore tessbinpath is not used.
				//not that PythonRankVerifierExecutable may fail here
				try
				{
					/*var weaponID = WeaponTable.WeaponIDCache[version].First(
									x => x.weaponNumber == weaponNumber &&
									(Categories)x.categoryNumber == categoryGroup
									).weaponID;*/


					_queue.Add(
						py.LoadOut(
							pythonExecutable.Construct(
								$"{(int)categoryGroup}_{weaponNumber}{temp}.png",
								imagePath,
								WeaponTable.WeaponIDAndCategoryWeaponNumbers[version].First(x => x.weaponID.Category == categoryGroup && x.weaponNumber == weaponNumber).weaponID,
								WeaponUtilityClass.GetWeaponType(categoryGroup),
								programDirectory,
								_isDefaultConversion
							)
						)
					);
				}
				catch (InvalidOperationException) //when something is missing from the weaponIDCache list
				{
					continue; //skip, it's not found g
				}
			}
		}



		/*
		if (version.IsLegacy)
		{
			if(pythonExecutable is PythonTesseractExecutable pyt) {
				_queue.Add(py
					.LoadOut(pyt
						.Construct($"{(int)categoryGroup}_{weaponNumber}_1.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath, _isDefaultConversion)));
				_queue.Add(py
					.LoadOut(pyt
						.Construct($"{(int)categoryGroup}_{weaponNumber}_2.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath, _isDefaultConversion)));
			}
			else 
			{
				_queue.Add(py
					.LoadOut(pythonExecutable
						.Construct($"{(int)categoryGroup}_{weaponNumber}_1.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, _isDefaultConversion)));
				_queue.Add(py
					.LoadOut(pythonExecutable
						.Construct($"{(int)categoryGroup}_{weaponNumber}_2.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, _isDefaultConversion)));
			}


		}else if(version.MultipleScreenshotsCheck() == 2 && !version.IsLegacy)
		{
			if (pythonExecutable is PythonTesseractExecutable pyt)
			{
				_queue.Add(py
					.LoadOut(pyt
						.Construct($"{(int)categoryGroup}_{weaponNumber}_0.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath, _isDefaultConversion)));
				_queue.Add(py
					.LoadOut(pyt
						.Construct($"{(int)categoryGroup}_{weaponNumber}_1.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath, _isDefaultConversion)));
			}
			else
			{
				_queue.Add(py
					.LoadOut(pythonExecutable
						.Construct($"{(int)categoryGroup}_{weaponNumber}_0.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, _isDefaultConversion)));
				_queue.Add(py
					.LoadOut(pythonExecutable
						.Construct($"{(int)categoryGroup}_{weaponNumber}_1.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, _isDefaultConversion)));
			}
		}
		else
		{
			if (pythonExecutable is PythonTesseractExecutable pyt)
			{
				_queue.Add(py
					.LoadOut(pyt
						.Construct($"{(int)categoryGroup}_{weaponNumber}.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath, _isDefaultConversion)));
			}
			else
			{
				_queue.Add(py
					.LoadOut(pythonExecutable
						.Construct($"{(int)categoryGroup}_{weaponNumber}.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, _isDefaultConversion)));
			}
		}*/
	}


	/// <summary>
	/// Constructs the factory from pre-defined values.
	/// </summary>
	/// <param name="weaponNumbers">An <see cref="IDictionary{TKey, TValue}"/> object where <c>TKey</c> refers to the version of the weapons, and <c>TValue</c> contains a <see cref="Dictionary{TKey, TValue}"/> object where <c>TKey</c> refers to the category of the specified version and <c>TValue</c> contains a list of the weapon numbers for the specified category and version .</param>
	/// <param name="versionAndPathPairs">An <see cref="IDictionary{TKey, TValue}"/> object where <c>TKey</c> refers to the Phantom Forces version, and <c>TValue</c> refers to the absolute path of where <c>TKey</c>'s version's images can be found.</param>
	/// <param name="programDirectory">The program directory of the Python executable.</param>
	/// <param name="outputDestination">Specifies the output destination.</param>
	/// <param name="tessbinPath">Specifies the absolute path of <c>/tessbin/</c> folder. If null, assumes such folder is in the same working directory.</param>
	/// <param name="coreCount">Specifies the core count manually. Default is dual (2) cores.</param>
	/// <param name="isDefaultConversion">Specifies if the images supplied are for default conversion.</param>
	/// <exception cref="DirectoryNotFoundException"/>
	public PythonExecutionFactory(IDictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>> weaponNumbers, IDictionary<PhantomForcesVersion, string> versionAndPathPairs, string programDirectory, OutputDestination outputDestination, string? tessbinPath, Cores coreCount = Cores.Dual, bool isDefaultConversion = true)
	{
		TPythonExecutable pythonExecutable = new();

		//checks if the directory is valid if not null
		if (Directory.Exists(tessbinPath) == false && tessbinPath != null && pythonExecutable is PythonTesseractExecutable)
		{
			PFDBLogger.LogFatal("The tessbin directory was not found, and was not null (null assumes that tessbinpath is in the same working directory)", parameter: tessbinPath);
			throw new DirectoryNotFoundException("The tessbin directory was not found, and was not null (null assumes that tessbinpath is in the same working directory). tessbinPath was " + tessbinPath);
		}

		//establish core count for performance reasons
		_coreCount = (int)coreCount;
		//populate queue
		_queue = new List<IPythonExecutor>();
		_isDefaultConversion = isDefaultConversion;

		//iterates through each version
		foreach (PhantomForcesVersion version in versionAndPathPairs.Keys)
		{
			try
			{
				//iterates through each category
				foreach (Categories categoryGroup in weaponNumbers[version].Keys)
				{
					//iterates through each weapon
					foreach (int weapon in weaponNumbers[version][categoryGroup])
					{
						_constructorHelper(outputDestination, categoryGroup, weapon, versionAndPathPairs[version], version, programDirectory, tessbinPath, pythonExecutable);
					}
				}
			}
			catch (KeyNotFoundException)
			{
				PFDBLogger.LogWarning($"The following key ({version} with version {version.VersionString}) was not found in IDictionary<PhantomForcesVersion,Dictionary<Categories,List<int>>> weaponNumbers");
				continue;
			}
		}
		_initialQueueCount = _queue.Count;
	}

	/// <summary>
	/// Constructs the factory from pre-defined values.
	/// </summary>
	/// <param name="weaponIDs">An <see cref="IDictionary{TKey, TValue}"/> object where <c>TKey</c> refers to the category of the weapons, and <c>TValue</c> contains the weapons' numbers (in the abovementioned category).</param>
	/// <param name="versionAndPathPairs">An <see cref="IDictionary{TKey, TValue}"/> object where <c>TKey</c> refers to the Phantom Forces version, and <c>TValue</c> refers to the absolute path of where <c>TKey</c>'s version's images can be found.</param>
	/// <param name="programDirectory">The program directory of the Python executable.</param>
	/// <param name="outputDestination">Specifies the output destination.</param>
	/// <param name="tessbinPath">Specifies the absolute path of <c>/tessbin/</c> folder. If null, assumes such folder is in the same working directory.</param>
	/// <param name="coreCount">Specifies the core count manually. Default is dual (2) cores.</param>
	/// <param name="isDefaultConversion">Specifies if the images supplied are for default conversion.</param>
	public PythonExecutionFactory(IDictionary<Categories, Collection<int>> weaponIDs, IDictionary<PhantomForcesVersion, string> versionAndPathPairs, string programDirectory, OutputDestination outputDestination, string? tessbinPath, Cores coreCount = Cores.Dual, bool isDefaultConversion = true)
	{
		TPythonExecutable pythonExecutable = new();
		if (Directory.Exists(tessbinPath) == false && tessbinPath != null && pythonExecutable is PythonTesseractExecutable) {
			PFDBLogger.LogFatal("The tessbin directory was not found, and was not null (null assumes that tessbinpath is in the same working directory)", parameter: tessbinPath);
			throw new DirectoryNotFoundException("The tessbin directory was not found, and was not null (null assumes that tessbinpath is in the same working directory). tessbinPath was " + tessbinPath);
		}
		_coreCount = (int)coreCount;
		_queue = new List<IPythonExecutor>();
		_isDefaultConversion = isDefaultConversion;
		foreach (PhantomForcesVersion version in versionAndPathPairs.Keys)
		{
			foreach (Categories categoryGroup in weaponIDs.Keys) {
				foreach (int weapon in weaponIDs[categoryGroup]) {
					if (File.Exists(versionAndPathPairs[version]) == false) {
						PFDBLogger.LogError("The targeted file was not found. Skipping.", parameter: versionAndPathPairs[version]);
						continue;
					}
					pythonExecutable = new();
					_constructorHelper(outputDestination, categoryGroup, weapon, versionAndPathPairs[version], version, programDirectory, tessbinPath, pythonExecutable);
				}
			}
		}
		_initialQueueCount = _queue.Count;
	}


	private StatusCounter _checkFactory()
	{
		if (_queue.Count == 0)
		{
			//only exception this function is allowed to throw directly back
			PFDBLogger.LogError("The internal list was empty.", parameter: nameof(_queue));
			throw new Exception("_queue was empty.");
		}
		StatusCounter checkStatus = new StatusCounter();
		for (int i = 0; i < _queue.Count; ++i)
		{
			try
			{
				_queue[i].Input.CheckInput();
				checkStatus.SuccessCounter++;
			}
			catch (Exception ex)
			{
				//check.Output = new FailedPythonOutput(ex.Message);
				//log

				PFDBLogger.LogError($"An exception was raised while checking the object. Internal Message: {ex.Message}", parameter: $"{_queue[i].Input.Filename}");
				_queue.Remove(_queue[i]);
				checkStatus.FailCounter++;
				continue; //skip
			}
		}
		return checkStatus;
	}

	/// <summary>
	/// Starts execution of the factory. Checks the factory to see if it is valid.
	/// <list type="bullet">
	///		<item>If invalid, this function will return a <see cref="IPythonExecutionFactoryOutput"/> with no successes and all fails for both checking and queuing.</item>
	///		<item>If valid, it continues as usual.</item>
	/// </list>
	/// Regardless of either case, <see cref="FactoryOutput"/> <b>will</b> be populated.
	/// <para>It then begins executing the list of executors that have been supplied. Execution is synchronous and multithreaded. Number of objects executed synchronously depends on the number of logical processors (cores) the machine has. It waits for the synchronized objects to succeed. If any objects have not succeeded in 5 minutes, the factory will bypass and continue to execute.</para>
	/// </summary>
	/// <inheritdoc/>
	public IPythonExecutionFactoryOutput Start()
	{


		StatusCounter CheckStatus = new StatusCounter();
		try
		{
			CheckStatus = _checkFactory();
		}
		catch (Exception ex)
		{
			//fail condition, _queue is empty

			int count = _queue.Count;
			if (ex.Message.Contains("empty")) {
				count = 1; //sets automatically to 1
			}
			_factoryOutput = new PythonExecutionFactoryOutput(_queue, _isDefaultConversion, CheckStatus, new StatusCounter(SuccessCounter: 0, FailCounter: count), new StatusCounter(SuccessCounter: 0, FailCounter: _queue.Count), new TimeSpan(0), 0, new TimeSpan(0), 0, missingFiles);
			return _factoryOutput;
		}
		StatusCounter QueueStatus = new StatusCounter();
		StatusCounter ExecutionStatus = new StatusCounter();

		PFDBLogger.LogInformation("Factory has started.");

		ThreadPool.SetMinThreads(_coreCount, _coreCount);
		ThreadPool.SetMaxThreads(_coreCount, _coreCount);

		List<List<IPythonExecutor>> listForQueue = new List<List<IPythonExecutor>>();
		if (_queue.Count < _coreCount) {
			//looks strange, but this code duplicates the list without affecting the actual _queue list
			listForQueue.Add(new List<IPythonExecutor>(_queue));
		}
		int i;
		// allocates items to a temporary queue buffer that restricts the number of parallel threads to be within the core count of the computer.
		for (i = 0; i < _queue.Count / _coreCount; i++)
		{
			List<IPythonExecutor> temp = new List<IPythonExecutor>();

			for (int j = 0; j < _coreCount; j++)
			{
				temp.Add(_queue[i * _coreCount + j]);
			}
			//listForQueue.Add(temp.ToList());
			listForQueue.Add([.. temp]);
		}
		if (_queue.Count % _coreCount != 0)
		{
			List<IPythonExecutor> temp1 = new List<IPythonExecutor>();
			for (int j = i * _coreCount; j < _queue.Count; ++j)
			{
				temp1.Add(_queue[j]);
			}
			listForQueue.Add(temp1);
		}

		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		DateTime start = DateTime.Now;
		TimeSpan totalParallelTimeElapsedDateTime = new TimeSpan(0);
		long totalParallelTimeElapsedInMilliseconds = 0;


		for (int currentListIndexInBigList = 0; currentListIndexInBigList < listForQueue.Count; ++currentListIndexInBigList)
		{
			//Console.WriteLine($"Thread Count: {ThreadPool.ThreadCount}, Completed Threads: {ThreadPool.CompletedWorkItemCount}, Pending Threads: {ThreadPool.PendingWorkItemCount}");

			int size = listForQueue[currentListIndexInBigList].Count;
			ManualResetEvent[] manualEvents = new ManualResetEvent[size];
			for (int smallListIndex = 0; smallListIndex < size; smallListIndex++)
			{
				if (listForQueue[currentListIndexInBigList][smallListIndex] is IAwaitable awaitable)
				{
					manualEvents[smallListIndex] = awaitable.ManualEvent;
				}
				try
				{
					//queues the job into the threadpool
					ThreadPool.QueueUserWorkItem(new WaitCallback(listForQueue[currentListIndexInBigList][smallListIndex].Execute));
					QueueStatus.SuccessCounter++;
				}
				catch (NotSupportedException e)
				{
					PFDBLogger.LogError(e.Message);
					QueueStatus.FailCounter++;
				}
			}

			//wait until all have finished. each has 5 minutes to finish before timing out
			if (WaitHandle.WaitAll(manualEvents, new TimeSpan(0, 5, 0)))
			{
				PFDBLogger.LogInformation("Current parallel asynchronous awaiter has finished.");
				//Console.WriteLine("success, all have been completed!");
				foreach (IPythonExecutor executor in listForQueue[currentListIndexInBigList])
				{
					if (executor.Output is Benchmark benchmark)
					{
						ExecutionStatus.SuccessCounter++;
						totalParallelTimeElapsedDateTime += benchmark.StopwatchDateTime;
						totalParallelTimeElapsedInMilliseconds += benchmark.StopwatchNormal.ElapsedMilliseconds;

					} else if (executor.Output is FailedPythonOutput failedOutput)
					{
						ExecutionStatus.FailCounter++;
						PFDBLogger.LogError($"Execution failed: Execution of the individual Python script failed", parameter: failedOutput.OutputString);
					}
					else
					{
						ExecutionStatus.SuccessCounter++;

					}
				}
			}
			else
			{
				PFDBLogger.LogError("The current parallel asynchronous awaiter timed out.");
				//Console.WriteLine("oh no, part 2 electric boogaloo");
				ExecutionStatus.FailCounter++;
			}
			//idk if this does anything, but i'm thinking it's a good idea to free resources
			/*foreach(ManualResetEvent u in manualEvents)
			{
				u.Dispose();
			}*/


			PFDBLogger.LogInformation($"Thread Count: {ThreadPool.ThreadCount}, Completed Threads: {ThreadPool.CompletedWorkItemCount}, Pending Threads: {ThreadPool.PendingWorkItemCount} {Environment.NewLine} Completed/Total Parallel Jobs: {currentListIndexInBigList + 1} / {listForQueue.Count}");

		}
		DateTime end = DateTime.Now;
		stopwatch.Stop();
		_factoryOutput = new PythonExecutionFactoryOutput(_queue, _isDefaultConversion, CheckStatus, QueueStatus, ExecutionStatus, totalParallelTimeElapsedDateTime, totalParallelTimeElapsedInMilliseconds, end - start, stopwatch.ElapsedMilliseconds, missingFiles);
		return _factoryOutput;

	}


}