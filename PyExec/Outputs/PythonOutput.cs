using PFDB.PythonExecutionUtility;

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
        OutputString = outputString;
    }

    /// <inheritdoc/>
    public new string ToString()
    {
        return OutputString;
    }
}