using PFDB.PythonExecutionUtility;
using PFDB.PythonFactoryUtility;

namespace PFDB.PythonFactory;


/// <summary>
/// Interface for Python execution factories.
/// </summary>
public interface IPythonExecutionFactory<TPythonExecutable>
{
	/// <inheritdoc cref="IPythonExecutionFactoryOutput.IsDefaultConversion"/>
	bool IsDefaultConversion { get; }

	/// <summary>
	/// The factory output which also includes several benchmarks. May be null.
	/// </summary>
	IPythonExecutionFactoryOutput? FactoryOutput { get; }

	/// <summary>
	/// Starts the factory, and executes the list of <see cref="IPythonExecutor"/>.
	/// Populates <see cref="FactoryOutput"/> with the result.
	/// </summary>
	/// <returns>A <see cref="IPythonExecutionFactoryOutput"/> object that contains the factory output as well as several benchmarks.</returns>
	IPythonExecutionFactoryOutput Start();
}