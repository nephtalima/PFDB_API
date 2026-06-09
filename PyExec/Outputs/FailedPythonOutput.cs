using System.Collections.Generic;
using PFDB.Logging;
using PFDB.PythonExecutionUtility;

namespace PFDB.PythonExecution;


/// <summary>
/// Implementation of <see cref="IOutput"/> meant to represent a failed execution.
/// </summary>
internal sealed class FailedPythonOutput : IOutput
{

    /// <summary>
    /// Output string.
    /// </summary>
    public string OutputString { get; init; }


    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="outputString">Output string from result.</param>
    public FailedPythonOutput(string outputString)
    {

		PFDBLogger.LogArguments(new Dictionary<string, object?>()
		{
            {nameof(outputString), outputString}
		});

		OutputString = outputString;
    }

    /// <inheritdoc/>
    public new string ToString()
    {
        PFDBLogger.LogArguments(new Dictionary<string, object?>(){});

        return OutputString;
    }
}