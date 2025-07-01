// See https://aka.ms/new-console-template for more information
using PFDB.PythonExecutionUtility;
using System;
using System.Diagnostics;

namespace PFDB.PythonExecution;


/// <summary>
/// Class that benchmarks how fast certain operations are finished, and inherits from <see cref="IOutput"/>.
/// </summary>
public sealed class Benchmark : IOutput
{
	private string outputStr;
	/// <summary>
	/// Main output string.
	/// </summary>
	public string OutputString { get { return outputStr; } }

	/// <summary>
	/// Stopwatch calculated with the <see cref="Stopwatch"/> class, returned as <see cref="Stopwatch"/>.
	/// </summary>
	public Stopwatch StopwatchNormal { get { return stopwatch; } }

	private TimeSpan stopwatchDateTime;
	/// <summary>
	/// Stopwatch calculated with the <see cref="TimeSpan"/> class, returned as <see cref="TimeSpan"/>.
	/// </summary>
	public TimeSpan StopwatchDateTime { get { return stopwatchDateTime; } }

	private DateTime start;
	/// <summary>
	/// Start time used to calculate <see cref="StopwatchDateTime"/>, returned as <see cref="TimeSpan"/>.
	/// </summary>
	public DateTime Start { get { return start; } }

	private DateTime end;
	/// <summary>
	/// End time used to calculate <see cref="StopwatchDateTime"/>, returned as <see cref="TimeSpan"/>.
	/// </summary>
	public DateTime End { get { return end; } }

	private Stopwatch stopwatch;

	/// <summary>
	/// Default constructor. Initializes all fields to zero or default values.
	/// </summary>
	public Benchmark()
	{
		start = DateTime.Now;
		end = start;
		stopwatch = new Stopwatch();
		stopwatchDateTime = TimeSpan.Zero;
		outputStr = string.Empty;
	}

	/// <summary>
	/// Starts both of the stopwatches.
	/// </summary>
	public void StartBenchmark()
	{
		start = DateTime.Now;
		stopwatch = Stopwatch.StartNew();
	}

	/// <summary>
	/// Stops both of the stopwatches, and assigns the results to <see cref="StopwatchNormal"/> and <see cref="StopwatchDateTime"/>.
	/// </summary>
	public void StopBenchmark()
	{
		end = DateTime.Now;
		stopwatch.Stop();
		stopwatchDateTime = (end - start);

	}

	/// <summary>
	/// Returns the amount of time elapsed by the stopwatches.
	/// </summary>
	/// <returns>A two-tuple, where the first value is calculated by <see cref="StopwatchDateTime"/> and the second by <see cref="StopwatchNormal"/>.</returns>
	public Tuple<double, double> GetElapsedTimeInSeconds()
	{
		return Tuple.Create(stopwatchDateTime.TotalSeconds, (double)stopwatch.ElapsedMilliseconds / (double)1000);
	}

	/// <summary>
	/// Returns the amount of time elapsed by the stopwatches.
	/// </summary>
	/// <param name="outputString">The output string to populate <see cref="OutputString"/>.</param>
	/// <returns>A two-tuple, where the first value is calculated by <see cref="StopwatchDateTime"/> and the second by <see cref="StopwatchNormal"/>.</returns>
	public Tuple<double, double> GetElapsedTimeInSeconds(string outputString)
	{
		this.outputStr = outputString;
		return Tuple.Create(stopwatchDateTime.TotalSeconds, (double)stopwatch.ElapsedMilliseconds / (double)1000);
	}

	/// <inheritdoc/>
	public new string ToString()
	{
		return OutputString;
	}
}