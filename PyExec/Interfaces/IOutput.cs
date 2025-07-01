namespace PFDB.PythonExecutionUtility;

/// <summary>
/// Interface for output types from classes inheriting from <see cref="IPythonExecutable"/>.
/// </summary>
public interface IOutput
{
    /// <summary>
    /// Output string.
    /// </summary>
    public string OutputString { get; }

    /// <summary>
    /// Overload of default <see cref="object.ToString()"/> method.
    /// </summary>
    /// <returns>The value contained in <see cref="OutputString"/></returns>
    public string ToString();
}