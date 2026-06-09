using PFDB.PythonExecutionUtility;
using PFDB.Logging;
using System.Collections.Generic;

namespace PFDB.PythonExecution;


/// <summary>
/// Default implementation of <see cref="IOutput"/>.
/// </summary>
public class PythonOutput : IOutput
{
    /// <summary>
    /// Output string.
    /// </summary>
    public string OutputString { get; init; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="outputString">Output string from result.</param>
    public PythonOutput(string outputString)
    {
        
        PFDBLogger.LogArguments(new Dictionary<string, object?>(){
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