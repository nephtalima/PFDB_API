using PFDB.Parsing;
using System;
using PFDB.StatisticStructure;
using PFDB.WeaponUtility;
using PFDB.ConversionUtility;
using PFDB.PythonExecutionUtility;
using static PFDB.Parsing.DefaultStatisticParameters;

namespace PFDB.Conversion;

/// <summary>
/// Defines a single conversion for weapons.
/// </summary>
public class Conversion : IConversion
{
	private protected WeaponIdentification _WID;
	private protected int _conversionID = 0;
	private protected IStatisticCollection _statisticCollection;


	/// <inheritdoc/>
	public IStatisticCollection StatisticCollection { get { return _statisticCollection; } }

	/// <inheritdoc/>
	public WeaponIdentification WeaponID { get { return _WID; } }

	/// <inheritdoc/>
	public WeaponType WeaponType => _WID.WeaponType;

	/// <inheritdoc/>
	public bool NeedsRevision => _statisticCollection.CollectionNeedsRevision;

	/// <summary>
	/// Constructor with a collection of statistics. All fields are populated.
	/// </summary>
	/// <param name="statisticCollection">A collection of statistics for the current conversion.</param>
	public Conversion(IStatisticCollection statisticCollection)
	{
		_statisticCollection = statisticCollection;
		_WID = statisticCollection.WeaponID;
	}

	/// <summary>
	/// Constructor that requires a path to a file that contains the statistics (typically a file that ends in .pfdb), and unique weapon identifier (<see cref="WeaponIdentification"/>). Populates all fields.
	/// Invokes <see cref="IFileParse.FileReader(string)"/> to open the file, and <see cref="IFileParse.FindAllStatisticsInFileWithTypes(int, int, StringComparison, bool)"/> to extract the statistics.
	/// </summary>
	/// <param name="filepath">Path to the file that contains the statistics (typically a file that ends in .pfdb).</param>
	/// <param name="weaponID">Unique weapon identifier for the weapon that contains the conversion.</param>
	public Conversion(string filepath, WeaponIdentification weaponID)
	{
		_WID = weaponID;
		IFileParse fileToBeParsed = new FileParse(weaponID);
		fileToBeParsed.FileReader(filepath);
		_statisticCollection = fileToBeParsed.FindAllStatisticsInFileWithTypes(AcceptableCorruptedWordSpaces, AcceptableSpaces, StringComparisonMethod); //todo: pass default params to this ctor

	}

	/// <summary>
	/// Constructor that accepts a single <see cref="IPythonExecutor"/> object. Populates all fields of this class.
	/// If <see cref="IPythonExecutor.Output"/> is empty, this function calls <see cref="IPythonExecutor.Execute(object?)"/> to populate it.
	/// Invokes <see cref="IFileParse.FindAllStatisticsInFileWithTypes(int, int, StringComparison, bool)"/> to extract the statistics.
	/// </summary>
	/// <param name="pythonExecutor"></param>
	public Conversion(IPythonExecutor pythonExecutor)
	{
		_WID = pythonExecutor.Input.WeaponID;
		if (pythonExecutor.HasExecuted == false) pythonExecutor.Execute(null);
		IFileParse fileToBeParsed = new FileParse(_WID, pythonExecutor.Output.ToString());
		

		_statisticCollection = fileToBeParsed.FindAllStatisticsInFileWithTypes(AcceptableCorruptedWordSpaces, AcceptableSpaces, StringComparisonMethod); //todo: pass default params to this ctor

		//todo
		if (pythonExecutor.DefaultConversion)
			_conversionID = 0;
	}
	
}
