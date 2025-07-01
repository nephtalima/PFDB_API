using PFDB.WeaponUtility;
using System;
using System.Diagnostics;
using System.IO;

namespace PFDB.PythonExecutionUtility;


/// <summary>
/// Interface that defines interactions with the Python script.
/// </summary>
public interface IPythonExecutable
{
	/// <summary>
	/// The filename of the image to pass to the executor.
	/// </summary>
	public string Filename { get; }

	/// <summary>
	/// Determines whether the current object is the default conversion for the weapon.
	/// </summary>
	public bool IsDefaultConversion { get; }

	/// <summary>
	/// Directory where the Python executable resides
	/// </summary>
	public string ProgramDirectory { get; }

	/// <summary>
	/// The unique weapon identifier for the weapon being processed.
	/// </summary>
	public WeaponIdentification WeaponID { get; }

	/// <summary>
	/// WeaponType of the weapon, telling the Python application what type of weapon to read.
	/// </summary>
	public WeaponType WeaponType { get; }

	/// <summary>
	/// Constructs the <see cref="ProcessStartInfo"/> object. This object is used to tell the program what commands it needs to run. This function is what invokes the Python script. Do note that this function currently works with just Windows.
	/// </summary>
	/// <returns>A <see cref="ProcessStartInfo"/> object that can be executed to read the image specified by <see cref="Filename"/></returns>
	public ProcessStartInfo GetProcessStartInfo();

	/// <summary>
	/// Checks if the parameters passed through <see cref="IPythonExecutable.Construct(string, string, WeaponIdentification, WeaponType, string, bool)"/> are valid. Throws any one of the following errors if the parameters are invalid.
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	/// <exception cref="PythonAggregateException"></exception>
	public void CheckInput();


	/// <summary>
	/// Returns the output.
	/// </summary>
	/// <returns>The <see cref="IOutput"/> object associated with the current class.</returns>
	public IOutput ReturnOutput();

	/// <summary>
	/// Constructs the executable. This method populates all the fields within this class.
	/// </summary>
	/// <param name="filename">Name of the file to be read by the Python application.</param>
	/// <param name="fileDirectory">Directory where the images for reading reside.</param>
	/// <param name="weaponType">WeaponType of the weapon, telling the Python application where to read.</param>
	/// <param name="weaponID">Phantom Forces weapon identification.</param>
	/// <param name="programDirectory">Directory where the Python executable resides.</param>
	/// <param name="isDefaultConversion">Specifies if the images supplied are for default conversion.</param>
	/// <returns>A reference to this class for use in anything inheriting from <see cref="IPythonExecutor"/>.</returns>
	public IPythonExecutable Construct(string filename, string fileDirectory, WeaponIdentification weaponID, WeaponType weaponType, string programDirectory, bool isDefaultConversion = true);
}
