using PFDB.Logging;
using PFDB.PythonExecutionUtility;
using PFDB.PythonFactoryUtility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace PFDB.PythonFactory;

/// <summary>
/// Defines a factory output that contains the output from <see cref="PythonExecutionFactory{TPythonExecutable}"/>. 
/// Also includes various status counters which indicate the success rate of various stages of <see cref="PythonExecutionFactory{TPythonExecutable}"/>.
/// Additionally, includes benchmarks on execution times for <see cref="PythonExecutionFactory{TPythonExecutable}"/>.
/// </summary>
public sealed class PythonExecutionFactoryOutput : IPythonExecutionFactoryOutput
{
	/// <inheritdoc/>
	public IEnumerable<IPythonExecutor> PythonExecutors { get;  }

	/// <inheritdoc/>
	public StatusCounter CheckStatusCounter { get; }

	/// <inheritdoc/>
	public StatusCounter QueueStatusCounter { get; }

	/// <inheritdoc/>
	public StatusCounter ExecutionStatusCounter { get; }

	/// <inheritdoc/>
	public TimeSpan TotalParallelExecutionTimeFromDateTime { get; }

	/// <inheritdoc/>
	public long TotalParallelExecutionTimeFromStopwatchInMilliseconds { get; }

	/// <inheritdoc/>
	public TimeSpan ActualExecutionTimeFromDateTime { get; }

	/// <inheritdoc/>
	public long ActualExecutionTimeFromStopwatchInMilliseconds { get; }

	/// <inheritdoc/>
	public bool IsDefaultConversion { get; }

	/// <inheritdoc/>
	public IEnumerable<string> MissingFiles {get;}

	/// <summary>
	/// Default constructor.
	/// </summary>
	/// <param name="pythonExecutors">The list of internal <see cref="IPythonExecutor"/> objects.</param>
	/// <param name="isDefaultConversion">Indicates whetheer the <see cref="IPythonExecutor"/> objects are used for the default conversion.</param>
	/// <param name="checkStatusCounter">The counter for the number of items that pass or fail <see cref="PythonExecutionFactory{TPythonExecutable}._checkFactory"/>.</param>
	/// <param name="queueStatusCounter">The counter for the number of items that pass or fail being queued via <see cref="ThreadPool.QueueUserWorkItem(WaitCallback)"/>.</param>
	/// <param name="executionStatusCounter">The counter for the number of items that pass or fail during execution from <see cref="IPythonExecutor.Execute(object?)"/>.</param>
	/// <param name="totalParallelExecutionTimeFromDateTime">Total parallel execution time of the entire factory (across all threads). Calculated with <see cref="DateTime"/>.</param>
	/// <param name="totalParallelExecutionTimeFromStopwatchInMilliseconds">Total parallel execution time of the entire factory (across all threads) in milliseconds. Calculated with <see cref="Stopwatch"/>.</param>
	/// <param name="actualElapsedExecutionTimeFromDateTime">Actual serial execution time of the entire factory (on main thread). Calculated with <see cref="DateTime"/>.</param>
	/// <param name="actualExecutionTimeFromStopwatchInMilliseconds">Actual serial execution time of the entire factory (on main thread). Calculated with <see cref="Stopwatch"/>.</param>
	/// <param name="missingFiles">Missing files that the factory expected.</param>
	public PythonExecutionFactoryOutput(IEnumerable<IPythonExecutor> pythonExecutors,bool isDefaultConversion, StatusCounter checkStatusCounter, StatusCounter queueStatusCounter, StatusCounter executionStatusCounter, TimeSpan totalParallelExecutionTimeFromDateTime, long totalParallelExecutionTimeFromStopwatchInMilliseconds, TimeSpan actualElapsedExecutionTimeFromDateTime, long actualExecutionTimeFromStopwatchInMilliseconds, IEnumerable<string> missingFiles)
	{
		IsDefaultConversion = isDefaultConversion;
		PythonExecutors = pythonExecutors;
		CheckStatusCounter = checkStatusCounter;
		QueueStatusCounter = queueStatusCounter;
		ExecutionStatusCounter = executionStatusCounter;
		TotalParallelExecutionTimeFromDateTime = totalParallelExecutionTimeFromDateTime;
		TotalParallelExecutionTimeFromStopwatchInMilliseconds = totalParallelExecutionTimeFromStopwatchInMilliseconds;
		ActualExecutionTimeFromDateTime = actualElapsedExecutionTimeFromDateTime;
		ActualExecutionTimeFromStopwatchInMilliseconds = actualExecutionTimeFromStopwatchInMilliseconds;
		MissingFiles = missingFiles;

		PFDBLogger.LogInformation(
			$"\t\t{Environment.NewLine}Datetime parallel time: {TotalParallelExecutionTimeFromDateTime.TotalMilliseconds} ms" +
			$"\tStopwatch parallel time {TotalParallelExecutionTimeFromStopwatchInMilliseconds} ms {Environment.NewLine}" +
			$"\t\tDatetime serial time {ActualExecutionTimeFromDateTime.TotalMilliseconds} ms " +
			$"\tStopwatch serial time {ActualExecutionTimeFromStopwatchInMilliseconds} {Environment.NewLine}" +
			$"\t\tCheck successes {CheckStatusCounter.SuccessCounter} " +
			$"\t\tCheck failures {CheckStatusCounter.FailCounter} {Environment.NewLine}" +
			$"\t\tQueue successes {QueueStatusCounter.SuccessCounter} " +
			$"\t\tQueue failures {QueueStatusCounter.FailCounter} {Environment.NewLine}" +
			$"\t\tExecution successes {ExecutionStatusCounter.SuccessCounter} " +
			$"\t\tExecution failures {ExecutionStatusCounter.FailCounter} "
		);
	}

	/// <inheritdoc/>
	public new string ToString()
	{
		StringBuilder builder = new StringBuilder();
		foreach(IPythonExecutor pyt in PythonExecutors)
			builder.Append( $"{pyt.ToString()}, {Environment.NewLine}" );
		return builder.ToString();
	}
}

