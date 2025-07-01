using PFDB.PythonExecutionUtility;
using PFDB.PythonFactory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PFDB.PythonFactoryUtility;
        
/// <summary>
/// Defines an interface that contains the output from <see cref="PythonExecutionFactory{TPythonExecutable}"/>. 
/// Also includes various status counters which indicate the success rate of various stages of <see cref="PythonExecutionFactory{TPythonExecutable}"/>.
/// Additionally, includes benchmarks on execution times for <see cref="PythonExecutionFactory{TPythonExecutable}"/>.
/// </summary>
public interface IPythonExecutionFactoryOutput
{

    /// <summary>
    /// The list of internal <see cref="IPythonExecutor"/> objects.
    /// </summary>
    IEnumerable<IPythonExecutor> PythonExecutors { get; }

    /// <summary>
    /// The counter for the number of items that pass or fail <see cref="PythonExecutionFactory{TPythonExecutable}._checkFactory"/>.
    /// </summary>
    StatusCounter CheckStatusCounter { get; }

    /// <summary>
    /// The counter for the number of items that pass or fail being queued via <see cref="ThreadPool.QueueUserWorkItem(WaitCallback)"/>.
    /// </summary>
    StatusCounter QueueStatusCounter { get; }

    /// <summary>
    /// The counter for the number of items that pass or fail during execution from <see cref="IPythonExecutor.Execute(object?)"/>
    /// </summary>
    StatusCounter ExecutionStatusCounter { get; }

    /// <summary>
    /// Total parallel execution time of the entire factory (across all threads). Calculated with <see cref="DateTime"/>.
    /// </summary>
    TimeSpan TotalParallelExecutionTimeFromDateTime { get; }

    /// <summary>
    /// Total parallel execution time of the entire factory (across all threads) in milliseconds. Calculated with <see cref="Stopwatch"/>.
    /// </summary>
    long TotalParallelExecutionTimeFromStopwatchInMilliseconds { get; }

    /// <summary>
    /// Actual serial execution time of the entire factory (on main thread). Calculated with <see cref="DateTime"/>.
    /// </summary>
    TimeSpan ActualExecutionTimeFromDateTime { get; }

    /// <summary>
    /// Actual serial execution time of the entire factory (on main thread). Calculated with <see cref="Stopwatch"/>.
    /// </summary>
    long ActualExecutionTimeFromStopwatchInMilliseconds { get; }

    /// <summary>
    /// Indicates if the images supplied to <see cref="PythonExecutionFactory{TPythonExecutable}"/> are default conversions.
    /// </summary>
    bool IsDefaultConversion { get; }

    /// <summary>
    /// Rrturns a string concatenating every single element from <see cref="PythonExecutors"/>.
    /// </summary>
    /// <returns>A string concatenating every single element from <see cref="PythonExecutors"/>.</returns>
    string ToString();

    /// <summary>
    /// Missing files that the factory expected.
    /// </summary>
    IEnumerable<string> MissingFiles { get; }
}
