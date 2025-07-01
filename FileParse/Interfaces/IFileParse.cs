using System;
using System.IO;

using PFDB.WeaponUtility;
using PFDB.StatisticStructure;

namespace PFDB.Parsing;

/// <summary>
/// Interface for defining how to parse a .pfdb file.
/// </summary>
public interface IFileParse
{
	/// <summary>
	/// The unique weapon identifier of the file specified.
	/// </summary>
	public WeaponIdentification WeaponID { get; }


	/// <summary>
	/// Reads a file. Throws <see cref="ArgumentNullException"/> if filepath is null, and <see cref="FileNotFoundException"/> if the file does not exist.
	/// </summary>
	/// <param name="filepath">Path to specified file.</param>
	/// <returns>Returns <see cref="string.Empty"/> if the reading failed at all, otherwise returns the text content of the file.</returns>
	public string FileReader(string filepath);

	/// <summary>
	/// Finds all the statistics in a file.
	/// </summary>
	/// <param name="acceptableSpaces">Specifies the acceptable number spaces between both words. Default is set to 3.</param>
	/// <param name="acceptableCorruptedWordSpaces">Specifies the acceptable number spaces that a corrupted word can have. Default is set to 3.</param>
	/// <param name="consoleWrite">Set to true to print to the console, false otherwise.</param>
	/// <param name="stringComparisonMethod">Specifies the StringComparison method to be used.</param>
	/// <returns>Returns an <see cref="IStatisticCollection"/> which contains all of the statistics.</returns>
	public IStatisticCollection FindAllStatisticsInFileWithTypes(int acceptableSpaces, int acceptableCorruptedWordSpaces, StringComparison stringComparisonMethod, bool consoleWrite = false);
}

