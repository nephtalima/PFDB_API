namespace PFDB.PythonExecutionUtility;


/// <summary>
/// Wrapper interface that makes use of <see cref="IPythonExecutable"/> as input. Serves to handle output destinations, and also contains other metadata about the file output.
/// </summary>
public interface IPythonExecutor
{
	/// <summary>
	/// Output of the enclosed <see cref="IOutput"/> object. Will be populated if this executor has executed <see cref="Execute(object?)"/>.
	/// </summary>
	public IOutput Output { get; }

	/// <summary>
	/// Input for this enclosing class (<see cref="IPythonExecutor"/>). Will execute and return in <see cref="Output"/> as a class implementing <see cref="IOutput"/>.
	/// </summary>
	public IPythonExecutable Input { get; }

	/// <summary>
	/// Indicates if this object has executed <see cref="Execute(object?)"/>.
	/// </summary>
	public bool HasExecuted { get; }

	/// <summary>
	/// Indicates if this executor contains a default conversion.
	/// </summary>
	public bool DefaultConversion { get; }


	/// <summary>
	/// Executes the Python Executable.
	/// Populates <see cref="Output"/> when finished.
	/// </summary>
	public abstract void Execute(object? bs);

	/// <summary>
	/// Loads the <see cref="IPythonExecutable"/> object to be executed.
	/// </summary>
	/// <param name="input">The <see cref="IPythonExecutable"/> object.</param>
	public abstract void Load(IPythonExecutable input);

	/// <summary>
	/// Returns the string contained in <see cref="IOutput.OutputString"/>.
	/// </summary>
	/// <returns>The string contained in <see cref="IOutput.OutputString"/>.</returns>
	string ToString();
}
