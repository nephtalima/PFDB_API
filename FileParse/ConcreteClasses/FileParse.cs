using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PFDB.Logging;
using PFDB.ParsingUtility;
using PFDB.AutomaticProofreading;
using PFDB.StatisticUtility;
using PFDB.WeaponUtility;
using PFDB.StatisticStructure;

namespace PFDB.Parsing;

/// <summary>
/// This class handles the text from the files generated after PyTesseract
/// </summary>
public sealed class FileParse : IFileParse
{
	private string _filetext = string.Empty;
	private WeaponIdentification _WID;
	private string _filepath = string.Empty;
	private bool _fileSupplied = false;
	
	/// <summary>
	/// Returns the current handled text.
	/// </summary>
	public string FileText {get {return _filetext;}}

	/// <inheritdoc/>
	public WeaponIdentification WeaponID { get { return _WID; } }

	/// <summary>
	/// Constructor that only populates <see cref="WeaponID"/>. It is mandatory to call <see cref="FileReader(string)"/> to load a text filepath or the 
	/// </summary>
	/// <param name="weaponID">Specifies the weaponID for the text provided.</param>
	public FileParse(WeaponIdentification weaponID)
	{
		_WID = weaponID;
	}

	/// <summary>
	/// Constructor that populates <see cref="WeaponID"/> and <see cref="_filetext"/>.
	/// </summary>
	/// <param name="weaponID"></param>
	/// <param name="text"></param>
	public FileParse(WeaponIdentification weaponID, string text)
	{
		_filepath = "<no filepath>";
		_filetext = text;
		_WID = weaponID;
	}

	

	/// <inheritdoc/>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	public string FileReader(string filepath)
	{
		if (filepath == null)
		{
			PFDBLogger.LogError("File path specified cannot be null.", parameter: nameof(filepath));
			throw new ArgumentNullException(nameof(filepath), "File path specified cannot be null.");
		}
		if (File.Exists(filepath) == false) {
			PFDBLogger.LogError($"The path specified at {filepath} does not exist. Ensure that the directory and file exists, then try again.");
			throw new FileNotFoundException($"File not found.", filepath); 
		}
		string output;

		if (_filetext == string.Empty) //if there is already text, we do NOT want to overwrite it
		{
			try
			{
				output = File.ReadAllText(filepath);
				_filepath = filepath;
				_fileSupplied = true;
			}
			catch
			{
				output = string.Empty;
				_filepath = "<no filepath>";
			}
			_filetext = output;
		}
		else
		{
			_filepath = "<no filepath>";
			return _filetext;
		}

		return output;
	}



	/// <summary>
	/// Finds all the statistics in a file. 
	/// Repeatedly calls <see cref="IStatisticParse.FindStatisticInFile(SearchTargets, IEnumerable{char}, StringComparison)"/> for every valid <see cref="SearchTargets"/> that apply for the weapon. By default, only stops when a new line or carriage return character is found.
	/// <para>Special case for <see cref="SearchTargets.AmmoCapacity"/>: Attempts to read for both <see cref="StatisticOptions.ReserveCapacity"/> and <see cref="StatisticOptions.MagazineCapacity"/> and tries to convert to an integer. If integer conversion fails, <see cref="StatisticOptions.TotalAmmoCapacity"/> will be the concatenation of the abovementioned options. Note that if it succeeds, it will be the algebraic sum of both.</para>
	/// <para>If corrupted words are found through <see cref="StatisticParse._corruptedWordFixer(string?, StringComparison)"/> and fixed, this function will update the file accordingly. It will also update <see cref="_filepath"/>.</para>
	/// </summary>
	/// <inheritdoc/>
	public IStatisticCollection FindAllStatisticsInFileWithTypes(int acceptableSpaces, int acceptableCorruptedWordSpaces, StringComparison stringComparisonMethod, bool consoleWrite = false)
	{
		//IDictionary<SearchTargets, string> temp = new Dictionary<SearchTargets, string>();
		IStatisticParse statisticParse = new StatisticParse(_WID, _filetext, acceptableSpaces, acceptableCorruptedWordSpaces, consoleWrite);
		IStatisticCollection statistics = new StatisticCollection(_WID);

		foreach (SearchTargets target in ParsingUtilityClass.GetSearchTargetsForWeapon(_WID.WeaponType))
		{
			string filecopy = _filetext;
			try
			{
				StatisticProofread proofread = new StatisticProofread(_WID);

				// (char)13 represents carriage return, (char)10 represents Line Feed
				// these characters are the delimiters where the statistic reading will stop
				string statisticOutputLine = statisticParse.FindStatisticInFile(target, new List<char>() { 
					(char)13, (char)10 
				}, stringComparisonMethod);

				//if target is ammo capacity related, handle it manually
				if (target == SearchTargets.AmmoCapacity)
				{
					IStatistic magcap = proofread.ApplyRegularExpression(StatisticOptions.MagazineCapacity, statisticOutputLine);
					IStatistic rescap = proofread.ApplyRegularExpression(StatisticOptions.ReserveCapacity, statisticOutputLine);
					if (magcap.Statistics.Count() == 1 && rescap.Statistics.Count() == 1)
					{
						string magcapfirst = magcap.Statistics.First();
						string rescapfirst = rescap.Statistics.First();
						string res = string.Empty;
						bool needsRevision = false;

						//attempt to convert to integers
						try
						{
							int magcapint = Convert.ToInt32(magcapfirst);
							int rescapint = Convert.ToInt32(rescapfirst);
							res = (magcapint + rescapint).ToString();
						}
						catch
						{
							PFDBLogger.LogWarning("Magazine and/or reserve capacity could not be converted to integers", parameter: [magcap, rescap]);

							//if all else fails, at least get the string and combine it
							res = magcapfirst + ' ' + rescapfirst;
							needsRevision = true;
							continue;
						}
						
						//add three new statistics: magazine capacity, reserve capacity, and total capacity
						statistics.AddRange(
							[new Statistic(needsRevision, magcapfirst, magcap.WeaponID, magcap.Option),
								new Statistic(needsRevision, rescapfirst, magcap.WeaponID, rescap.Option), 
								new Statistic(needsRevision, res, proofread.WeaponID, StatisticOptions.TotalAmmoCapacity)]
							);
					}
				}
				else 
					//all other cases
					statistics.Add(proofread.ApplyRegularExpression(StatisticProofread.SearchTargetToStatisticOption(target), statisticOutputLine));
				
				_filetext = statisticParse.Filetext; //update, so corrupted words get fixed
			}
			catch(WordNotFoundException ex)
			{
				PFDBLogger.LogWarning($"An exception was raised while searching the text{(_fileSupplied ? " from " + _filepath : "")}. Internal Message: {ex.Message}", parameter: $"In {_filepath} searching for {target}");
				PFDBLogger.LogDebug($"{ex.StackTrace}");
			}
			catch (ArgumentException)
			{
				continue;
			}
			catch(Exception ex)
			{
				PFDBLogger.LogWarning($"An exception was raised while searching the text{(_fileSupplied ? " from " + _filepath : "")}. Internal Message: {ex.Message}", parameter: $"In {_filepath} searching for {target}");
				PFDBLogger.LogDebug($"{ex.StackTrace}");
				//do nothing
			}

			if (statistics.IsMissingStatistic)
			{
				StringBuilder builder = new StringBuilder();
				foreach (IStatistic r in statistics.Statistics)
					builder.Append($"{r.Option.ToString()}\t");
				PFDBLogger.LogWarning($"Missing the following statistic types {builder.ToString()}");
			}


			//only update if changed, otherwise just keep it the same
			if(filecopy != _filetext && _fileSupplied)
				File.WriteAllText(_filepath, _filetext);
		}
		return statistics;
	}
}