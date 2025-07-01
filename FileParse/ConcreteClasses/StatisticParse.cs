using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using PFDB.ParsingUtility;
using PFDB.Logging;
using PFDB.WeaponUtility;

namespace PFDB.Parsing;
/// <summary>
/// Parses individual statistic from the text provided.
/// </summary>
public sealed class StatisticParse : IStatisticParse
{

	private WeaponIdentification _WID;

	private SearchTargets _searchTarget = 0;
	private List<IIndexSearch> _wordLocationSearchers = new List<IIndexSearch>();
	private string _filetext = string.Empty;
	private List<string> _inputWordList = new List<string>();
	IEnumerable<int> _statisticNonGrataLocations = new List<int>();

	private bool _consoleWrite;
	private int _acceptableSpaces;
	private int _acceptableCorruptedWordSpaces; //margin of error
	
	/// <inheritdoc/>
	public WeaponIdentification WeaponID { get { return _WID; } }

	/// <summary>
	/// Returns the text that this class is working with.
	/// </summary>
	public string Filetext { get { return _filetext; } }

	/// <summary>
	/// Default constructor. Populates all necessary fields with the parameters passed through.
	/// </summary>
	/// <param name="weaponID">The Phantom Forces Version.</param>
	/// <param name="text">The text to search through</param>
	/// <param name="acceptableSpaces">Specifies the acceptable number spaces between words. Default is set to 3.</param>
	/// <param name="acceptableCorruptedWordSpaces">Specifies the acceptable number spaces that a corrupted word can have. Default is set to 3.</param>
	/// <param name="consoleWrite"></param>
	public StatisticParse(WeaponIdentification weaponID, string text, int acceptableSpaces = 3, int acceptableCorruptedWordSpaces = 3, bool consoleWrite = false)
	{

		_WID = weaponID;
		_consoleWrite = consoleWrite;
		_acceptableCorruptedWordSpaces = acceptableCorruptedWordSpaces;
		_acceptableSpaces = acceptableSpaces;
		_filetext = text;
		//_currentPosition = 0;
	}

	/// <summary>
	/// Finds a statistic defined in <see cref="SearchTargets"/> within the text provided in the constructor.
	/// <para>Keep in mind this function calls <see cref="_corruptedWordFixer(string?, StringComparison)"/>, and as such will alter the file (done by <see cref="IFileParse.FindAllStatisticsInFileWithTypes(int, int, StringComparison, bool)"/>).</para>
	/// This function does the following:
	/// <list type="number">
	///		<item>Clears both the input word list and the index searching list.</item>
	///		<item>Populates the input word list (<see cref="_inputWordList"/>) and word locations (<see cref="_wordLocationSearchers"/>) based on <c>target</c>.</item>
	///		<item>Searches against similar statistics, and removes duplicates.</item>
	///		<item>Returns statistic at the end.</item>
	/// </list>
	/// </summary>
	/// <param name="target">Specifies the target to search for.</param>
	/// <param name="endings">Specifies the delimiters (or breakpoints) where the statistic will read until.</param>
	/// <param name="stringComparisonMethod">Specifies the StringComparison method to be used.</param>
	/// <returns>A string that contains the statistic name and the actual value itself.</returns>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="WordNotFoundException"></exception>
	public string FindStatisticInFile(SearchTargets target, IEnumerable<char> endings, StringComparison stringComparisonMethod)
	{
		_inputWordList.Clear();
		_wordLocationSearchers.Clear();

		for(int i = 0; i < _wordLocationSearchers.Count; ++i)
		{
			_wordLocationSearchers[i] = new IndexSearch("", null, stringComparisonMethod);
		}

		
		if (target <= SearchTargets.FireModes && 
		_WID.WeaponType != WeaponType.Primary && 
		_WID.WeaponType != WeaponType.Secondary && 
			(target >= SearchTargets.HeadMultiplier && 
			target <= SearchTargets.LimbMultiplier && 
			_WID.WeaponType == WeaponType.Melee) == false
			)
		{
			//return; //error
			PFDBLogger.LogWarning($"The SearchTarget specified does not match the WeaponType specified. SearchTarget: {target}, WeaponType: {_WID.WeaponType}. Weapon ID: {_WID.ID}, Weapon Name: {_WID.WeaponName}");
			throw new ArgumentException($"The SearchTarget specified does not match the WeaponType specified. SearchTarget: {target}, WeaponType: {_WID.WeaponType}. Weapon ID: {_WID.ID}, Weapon Name: {_WID.WeaponName}");
		}
		else if (target >= SearchTargets.BlastRadius && target <= SearchTargets.StoredCapacity && _WID.WeaponType != WeaponType.Grenade)
		{
			//return; //error
			PFDBLogger.LogWarning($"The SearchTarget specified does not match the WeaponType specified. SearchTarget: {target}, WeaponType: {_WID.WeaponType}. Weapon ID: {_WID.ID}, Weapon Name: {_WID.WeaponName}");
			throw new ArgumentException($"The SearchTarget specified does not match the WeaponType specified. SearchTarget: {target}, WeaponType: {_WID.WeaponType}. Weapon ID: {_WID.ID}, Weapon Name: {_WID.WeaponName}");
		}
		else if (target >= SearchTargets.FrontStabDamage && _WID.WeaponType != WeaponType.Melee)
		{
			//return; //error
			PFDBLogger.LogWarning($"The SearchTarget specified does not match the WeaponType specified. SearchTarget: {target}, WeaponType: {_WID.WeaponType}. Weapon ID: {_WID.ID}, Weapon Name: {_WID.WeaponName}");
			throw new ArgumentException($"The SearchTarget specified does not match the WeaponType specified. SearchTarget: {target}, WeaponType: {_WID.WeaponType}. Weapon ID: {_WID.ID}, Weapon Name: {_WID.WeaponName}");
		}

		if (_filetext == "")
		{ //return; // error
			PFDBLogger.LogWarning("The supplied text was empty.");
			throw new ArgumentException("The supplied text was empty.");
		}
		_searchTarget = target;

		_inputWordsSelection();


		StringBuilder result = new StringBuilder();
		List<char> endingsList = new List<char>(endings);
		endingsList.AddRange(new List<char>() { (char)13, (char)10 });
		// do NOT attempt to run _corruptedWordFixer here or you WILL regret it...

		foreach (IIndexSearch word in _wordLocationSearchers)
		{
			//Console.WriteLine(target);
			if (word.IsEmpty())
			{	
				_corruptedWordFixer(word.Word, stringComparisonMethod);
				word.Search();
			}
		}

		switch (target)
		{
			//one word cases
			case SearchTargets.Firerate:
			case SearchTargets.Walkspeed:
				{
					_oneWordCaseHander(stringComparisonMethod);
					break;
				}
			//special cases
			case SearchTargets.Damage:
				{
					//if legacy version, eliminate damage range instances
					if (_WID.Version.IsLegacy)
					{
						_oneWordCaseHander(stringComparisonMethod);
						_getStatisticNonGrataLocations(stringComparisonMethod, ["damage", "range"]);
					}
					break;
				}
			case SearchTargets.Suppression:
				{
					_oneWordCaseHander(stringComparisonMethod);
					try
					{
						_getStatisticNonGrataLocations(stringComparisonMethod, ["suppression", "range"]);
					}
					catch (WordNotFoundException)
					{
						//do nothing, it couldn't find "suppression range" so we good :D
					}
					break;
				}
			case SearchTargets.Rank:
				{
					_oneWordCaseHander(stringComparisonMethod);
					IIndexSearch rankinfoSearch = new IndexSearch(_filetext, "RankInfo", stringComparisonMethod);
					_wordLocationSearchers[0].RemoveFromList(rankinfoSearch.ListOfIndices);
					break;
				}
			case SearchTargets.ReloadTime:
				{
					_getStatisticNonGrataLocations(stringComparisonMethod, ["empty", "reload", "time"]);
					break;
				}

			//case where we have 2+ words
			default:
				{
					if (_wordLocationSearchers.Last().ListOfIndices.Count > 0) goto Success;
					// damage range will never be found in a new version
					if (target == SearchTargets.DamageRange && _WID.Version.IsLegacy == false) { goto Success; }
					/*
						* this exception is intentional 
						* i want to keep the filter tight so we don't have random garbage getting in,
						* if we don't have at least the last word, we can't be 100% sure that we have the exact statistic
						*/
					throw new WordNotFoundException($"None of the {_inputWordList.Count} words were found.");
				}
		}

		Success:

		List<int> locations = new List<int>(); //dummy endingsList
		try
		{
			// checks if using newer versions, in which case there is no "damage range" or "damage" written anywhere
			if (_WID.Version.IsLegacy == false && 
				(_searchTarget == SearchTargets.DamageRange || _searchTarget == SearchTargets.Damage)) 
					goto DamageRangeSkip; //skip because we expect it not to be there

			locations = _grabStatisticLocations(stringComparisonMethod).ToList();
		}
		catch
		{
			PFDBLogger.LogWarning("None of the two words (\"damage\" or \"range\") were found.");
			throw new WordNotFoundException("None of the two words (\"damage\" or \"range\") were found.");
			//return; //error, no words were found
		}

	DamageRangeSkip:

		//removes locations that we don't care about
		locations = _removeStatisticNonGrataLocations(locations);


		//for newer versions, get the python index thing output (code looks like this in impa.py):
		/*
			*	// ...
			*	elif cropname == "DamageInfo" or cropname == "DamageAdvanced":
			*		data = ""
			*		for roi in rois:
			*			data += ("index " + str(roi[0]) + ": ")
			*			data += pytesseract.image_to_string(roi[1], config="--psm 7")
			*		return data
			*	// more code...
			* 
			*/
		if ((_searchTarget == SearchTargets.Damage ||  _searchTarget == SearchTargets.DamageRange) && _WID.Version.IsLegacy == false)
		{
			IIndexSearch indexSearch = new IndexSearch(_filetext, "index ", stringComparisonMethod);
			locations.AddRange(indexSearch.ListOfIndices);
		}

		List<string> statisticsList = new List<string>();

		// reads from the start of each location to any character specified in endingsList
		foreach (int location in locations)
		{ 
			StringBuilder r = new StringBuilder(string.Empty);

			for (int i = location; i < _filetext.Length; ++i)
			{
				if (endingsList.Contains(_filetext[i]) == false)
				{
					char test = _filetext[i];
					//technically this try wrapper can be removed, but its safer to leave it for now, im sick of debugging strange errors
					try
					{
						r.Append(_filetext[i]);
					}
					catch (ArgumentOutOfRangeException)
					{
						break;
					}
				}
				else
				{
					break;
				}
			}
			statisticsList.Add(r.ToString());
		}

		// looks for duplicates and removes them
		switch (statisticsList.Count)
		{
			case 0: throw new WordNotFoundException("No words were found.");
			case 1: break; // found one matching statistic, continue as regular
			default: //2 or more
			{
				for(int u = 0; u < statisticsList.Count; ++u)
				{
					for(int v = 0; v < statisticsList.Count; ++v)
					{
						if(statisticsList[u].Trim() == statisticsList[v].Trim() && u != v)
						{
							statisticsList.RemoveAt(int.Max(u, v));
								break; //i only expect one duplicate
						}
					}
				}
				break;
			}
		}

		foreach (string g in statisticsList)
			result.Append($"{g}\t");

		if(_consoleWrite) Console.WriteLine(result.ToString());
		return result.ToString();
	}

	/// <summary>
	/// Attempts to fix corrupted words.
	///		<para>
	///			Steps:
	///			<list type="number">
	///				<item>Iterate through all the first characters of the first word.</item>
	///				<item>Determine if something resembling the first word has been matched:
	///					<list type="bullet">
	///						<item>Skip over spaces</item>
	///						<item>See if <c>i</c> and <c>l</c> match (case-insensitive)</item>
	///					</list>
	///				</item>
	///				<item>Automatically replace detected corrupted words. <b>NOTE:</b> this happens to every word matched.</item>
	///			</list>
	///		</para>
	/// </summary>
	/// <param name="inputWord">Desired word to find and replace with.</param>
	/// <param name="stringComparisonMethod">Specifies the StringComparison method to be used.</param>
	/// <returns>Returns the corrupted word if it has been found, otherwise it returns <see cref="string.Empty"/></returns>
	internal string 
		_corruptedWordFixer(string? inputWord, StringComparison stringComparisonMethod)
	{
			if (inputWord == null) return string.Empty;
		List<int> wordFirstCharLocations = (List<int>)
											new IndexSearch(_filetext, inputWord.ToUpperInvariant()[0].ToString(), stringComparisonMethod, false)
											.Search();
		StringBuilder tempInputWord = new StringBuilder(inputWord);

		/////////////// double check this line, it may change, i might remove it ////////////////////
		// removed it because it removed actual legitimate entries
		//wordFirstCharLocations.RemoveAll((i) => i > _filetext.IndexOf("Does the file exist?", StringComparison.CurrentCultureIgnoreCase));
		wordFirstCharLocations.TrimExcess();

		string corruptedWord1 = ""; //to replace
		int letterMatch = 0;
		int actualSpaces = 0; //number of spaces detected when finding corrupted words
		foreach (int IndexI in wordFirstCharLocations) //through the list of indexes
		{
			/*
				* looks for i and l confusion, and matching letters
				* note: does this for every word matched, not just the "desired" one (why not fix the problem while we are at it?)
				* 
				* these two loops below start from the index in the loop above
				* the first loop starts from the index, and goes until the length of the word + buffer
				* the second loop starts from 0 and goes until the length of the word
				* we have a buffer, that way if there are spaces, the corrupted word (with spaces) can be replaced later
				* inside the innermost loop, there is a check to see if we have encountered a space, and if so, we can skip past
				* 		that character, but log it as a space. we do not want to check if that is part of the word since we have
				* 		not put spaces in the word definitions.
				* this way, when we get to the actual check later (to see if the lowercase versions of the letters are the same),
				* 		we are actually comparing letters, and not spaces. if there is a match, we increment letterMatch
				* when letterMatch equals the length of the word, we can then replace it with the correct word.
				*/
			//reset search variables
			tempInputWord = new StringBuilder(inputWord);
			//Console.WriteLine(_filetext.Substring(IndexI, tempInputWord.Length + _acceptableCorruptedWordSpaces));
			letterMatch = 0;
			actualSpaces = 0;

			for (int i = IndexI; i < IndexI + tempInputWord.Length + _acceptableCorruptedWordSpaces; i++) //for each char of the file at the chars location
			{
				if (i == -1) continue; //prevent indexOutOfRangeException
				for (int j = 0; j < tempInputWord.Length; j++) //through the word's character length
				{
					
					//skips over spaces and increments i only if we are still in the first word "region", otherwise is ignored
					if (_filetext[i] == 32 && i + 1 < IndexI + tempInputWord.Length + _acceptableSpaces)
					{
						actualSpaces++; //logs the number of spaces encountered
						i++;
						continue; //skips innermost loop if there is a space
					}

					/*testing variables, ideally do not remove*/
					//string location = filetext.Substring(i, 20); //1423
					//char testi = _filetext[i];
					//char testj = tempInputWord[j];
					//PFDBLogger.LogDebug($"_filetext[{i}] = {testi}, tempInputWord[{j}] = {testj}");
					

					//looks for matches, and sees if i is in a location where l is, and vice versa
					if (_filetext[i].ToString().ToLower() == tempInputWord[j].ToString().ToLower() ||
						(_filetext[i].ToString().ToLower() == "i" && tempInputWord[j].ToString().ToLower() == "l") ||
						(_filetext[i].ToString().ToLower() == "l" && tempInputWord[j].ToString().ToLower() == "i"))
					{
						letterMatch++;
						//i don't even know why i assigned index j to be ╚
						if (letterMatch <= inputWord.Length) tempInputWord[j] = (char)200;
						break;
					}
				}
			}

			//look through matching words, and automatically replace them
			if (letterMatch == tempInputWord.Length)
			{
				//find the actual corrupted word
				for (int i = IndexI; i < IndexI + tempInputWord.Length + actualSpaces; i++)
				{
					corruptedWord1 += _filetext[i];
				}
				//replace the whole word with the correct word, and add space padding after
				_filetext = _filetext.Replace(corruptedWord1.TrimEnd(), inputWord.ToUpper() + " ", StringComparison.CurrentCultureIgnoreCase);
				return corruptedWord1;
			}
		}
		return string.Empty;
	}

	/// <summary>
	/// Converts a string array into a list. (lol)
	/// </summary>
	/// <param name="words">Any string array.</param>
	/// <returns>A <see cref="List{T}"/> containing the elements from the string array.</returns>
	private static List<string> _lister(params string[] words)
	{
		return words.ToList();
	}

	/// <summary>
	/// Selects words based on target. See <see cref="SearchTargets"/> for the options.
	/// Also populates <see cref="_wordLocationSearchers"/> with the indices of the given words.
	/// </summary>
	private void _inputWordsSelection()
	{
		_inputWordList.Clear();

		switch (_searchTarget)
		{
			case SearchTargets.Rank:
				{
					_inputWordList.Add("rank"); break; 
				}

			//guns
			case SearchTargets.Damage:
				{
					//the space is intentional, i may have named something DamageInfo and i'm too lazy to change the python script rn, maybe a todo tbh
					_inputWordList.AddRange(_lister(["damage "]));
					//to be selected AGAINST
					break;
				}
			case SearchTargets.DamageRange:
				{
					_inputWordList.AddRange(_lister(["damage", "range"]));
					//to be selected FOR
					break;
				}
			case SearchTargets.Firerate:
				{ _inputWordList.AddRange(_lister(["firerate"])); break; }
			case SearchTargets.AmmoCapacity:
				{ _inputWordList.AddRange(_lister(["ammo", "capacity"])); ; break; }
			case SearchTargets.HeadMultiplier:
				{ _inputWordList.AddRange(_lister(["head", "multiplier"])); break; }
			case SearchTargets.TorsoMultiplier:
				{ _inputWordList.AddRange(_lister(["torso", "multiplier"]));  break; }
			case SearchTargets.LimbMultiplier:
				{ _inputWordList.AddRange(_lister(["limb", "multiplier"])); break; }
			case SearchTargets.MuzzleVelocity:
				{ _inputWordList.AddRange(_lister(["muzzle", "velocity"])); break; }
			case SearchTargets.Suppression:
				{ _inputWordList.AddRange(_lister(["suppression"])); ; break; }
			case SearchTargets.PenetrationDepth:
				{ _inputWordList.AddRange(_lister(["penetration", "depth"])); break; }
			case SearchTargets.ReloadTime:
				{ _inputWordList.AddRange(_lister(["reload", "time"])); break; }
			case SearchTargets.EmptyReloadTime:
				{ _inputWordList.AddRange(_lister(["empty", "reload", "time"])); break; }
			case SearchTargets.WeaponWalkspeed:
				{ _inputWordList.AddRange(_lister(["weapon", "walkspeed"])); break; }
			case SearchTargets.AimingWalkspeed:
				{ _inputWordList.AddRange(_lister(["aiming", "walkspeed"])); break; }
			case SearchTargets.AmmoType:
				{ _inputWordList.AddRange(_lister(["ammo", "type"])); break; }
			case SearchTargets.SightMagnification:
				{ _inputWordList.AddRange(_lister(["sight", "magnification"])); break; }
			case SearchTargets.MinimumTimeToKill:
				{ _inputWordList.AddRange(_lister(["minimum", "time", "to", "kill"])); break; }
			case SearchTargets.HipfireSpreadFactor:
				{ _inputWordList.AddRange(_lister(["hipfire", "spread", "factor"])); break; }
			case SearchTargets.HipfireRecoverySpeed:
				{ _inputWordList.AddRange(_lister(["hipfire", "recovery", "speed"])); break; }
			case SearchTargets.HipfireSpreadDamping:
				{ _inputWordList.AddRange(_lister(["hipfire", "spread", "damping"])); break; }
			case SearchTargets.HipChoke:
				{ _inputWordList.AddRange(_lister(["hip", "choke"])); break; }
			case SearchTargets.AimChoke:
				{ _inputWordList.AddRange(_lister(["aim", "choke"])); break; }
			case SearchTargets.EquipSpeed:
				{ _inputWordList.AddRange(_lister(["equip", "speed"])); break; }
			case SearchTargets.AimModelSpeed:
				{ _inputWordList.AddRange(_lister(["aim", "model", "speed"])); break; }
			case SearchTargets.AimMagnificationSpeed:
				{ _inputWordList.AddRange(_lister(["aim", "magnification", "speed"])); break; }
			case SearchTargets.CrosshairSize:
				{ _inputWordList.AddRange(_lister(["crosshair", "size"])); break; }
			case SearchTargets.CrosshairSpreadRate:
				{ _inputWordList.AddRange(_lister(["crosshair", "spread", "rate"])); break; }
			case SearchTargets.CrosshairRecoverRate:
				{ _inputWordList.AddRange(_lister(["crosshair", "recover", "rate"])); break; }
			case SearchTargets.FireModes:
				{ _inputWordList.AddRange(_lister(["fire", "modes"])); break; }

			//grenades
			case SearchTargets.BlastRadius:
				{ _inputWordList.AddRange(_lister(["blast", "radius"])); break; }
			case SearchTargets.KillingRadius:
				{ _inputWordList.AddRange(_lister(["killing", "radius"])); break; }
			case SearchTargets.MaximumDamage:
				{ _inputWordList.AddRange(_lister(["maximum", "damage"])); break; }
			case SearchTargets.TriggerMechanism:
				{ _inputWordList.AddRange(_lister(["trigger", "mechanism"])); break; }
			case SearchTargets.SpecialEffects:
				{ _inputWordList.AddRange(_lister(["special", "effects"])); break; }
			case SearchTargets.StoredCapacity:
				{ _inputWordList.AddRange(_lister(["stored", "capacity"])); break; }

			//melees
			case SearchTargets.FrontStabDamage:
				{ _inputWordList.AddRange(_lister(["front", "stab", "damage"])); break; }
			case SearchTargets.BackStabDamage:
				{ _inputWordList.AddRange(_lister(["back", "stab", "damage"])); break; }
			case SearchTargets.MainAttackTime:
				{ _inputWordList.AddRange(_lister(["main", "attack", "time"])); break; }
			case SearchTargets.MainAttackDelay:
				{ _inputWordList.AddRange(_lister(["main", "attack", "delay"])); break; }
			case SearchTargets.AltAttackTime:
				{ _inputWordList.AddRange(_lister(["alt", "attack", "time"])); break; }
			case SearchTargets.AltAttackDelay:
				{ _inputWordList.AddRange(_lister(["alt", "attack", "delay"])); break; }
			case SearchTargets.QuickAttackTime:
				{ _inputWordList.AddRange(_lister(["quick", "attack", "time"])); break; }
			case SearchTargets.QuickAttackDelay:
				{ _inputWordList.AddRange(_lister(["quick", "attack", "delay"])); break; }
			case SearchTargets.Walkspeed:
				{ _inputWordList.AddRange(_lister(["walkspeed"])); break; }
			default:
				{
					//prevent infinite loop
					break;
				}
		}

		_wordLocationSearchers.Clear();
		foreach(string word in _inputWordList)
		{
			_wordLocationSearchers.Add(new IndexSearch(_filetext, word));
		}

	}

	/// <summary>
	/// Searches if two words' index positions are close enough together <b>and</b> in order. 
	/// </summary>
	/// <param name="firstWordOrCharSearcher">The <see cref="IIndexSearch"/> implementation that searches for the first character or first word.</param>
	/// <param name="secondWordSearcher">The <see cref="IIndexSearch"/> implementation that searches for the second word.</param>
	/// <returns>A <see cref="IEnumerable{T}"/> that contains all the locations of the first word/character where the words are close enough and in order.</returns>
	private IEnumerable<int> _wordProximityChecker(IIndexSearch firstWordOrCharSearcher, IIndexSearch secondWordSearcher)
	{
		List<int> result = new List<int>();
		foreach (int i in firstWordOrCharSearcher.ListOfIndices)
		{
			foreach (int j in secondWordSearcher.ListOfIndices)
			{
				if (i + firstWordOrCharSearcher.Word?.Length + _acceptableSpaces > j && i < j)
				{
					result.Add(i);
				}
			}
		}
		return result;
	}

	/// <summary>
	/// Searches if two words' index positions are close enough together <b>and</b> in order. 
	/// </summary>
	/// <param name="firstWordOrCharLocations">The <see cref="IEnumerable{T}"/> list that contains the locations of the first character or first word.</param>
	/// <param name="secondWordLocations">The <see cref="IEnumerable{T}"/> list that contains the locations for the second word.</param>
	/// <param name="word">The first word. The content doesn't matter, just the length matters.</param>
	/// <returns>A <see cref="IEnumerable{T}"/> that contains all the locations of the first word/character where the words are close enough and in order.</returns>
	private IEnumerable<int> _wordProximityChecker(IEnumerable<int> firstWordOrCharLocations, IEnumerable<int> secondWordLocations, string word)
	{
		List<int> result = new List<int>();
		foreach (int i in firstWordOrCharLocations)
		{
			foreach (int j in secondWordLocations)
			{
				if (i + word.Length + _acceptableSpaces > j && i < j)
				{
					result.Add(i);
				}
			}
		}
		return result;
	}

	/// <summary>
	/// Searches the text to find locations where statistics are likely to be.
	/// </summary>
	/// <returns>An <see cref="IEnumerable{T}"/> containing the position of the first character of the first word.</returns>
	/// <param name="stringComparisonMethod">Specifies the StringComparison method to be used.</param>
	/// <exception cref="Exception"></exception>
	/// <exception cref="WordNotFoundException"></exception>
	private IEnumerable<int> _grabStatisticLocations(StringComparison stringComparisonMethod)
	{
		IEnumerable<int> ints;
		if(_wordLocationSearchers.Last().ListOfIndices.Count > 0)
		{
			if (_wordLocationSearchers.Count == 1)
				return _wordLocationSearchers[0].ListOfIndices;

			
			/*
				* offset since ints starts at original location, but we need an offset to make sure we aren't
				* too far away from the next word
				*/
			int offset = 0;
			if (_wordLocationSearchers.Count > 2)
			{
				offset = _inputWordList[0].Length + _inputWordList[1].Length;
			}
			
			//temporary list
			List<int> tempInts = new List<int>();

			//checks if ALL the searchers have one word found
			if (_wordLocationSearchers.All(x => x.ListOfIndices.Count > 0))
			{
				//2+ word case, check the first two words
				ints = _wordProximityChecker(_wordLocationSearchers[0], _wordLocationSearchers[1]);
				tempInts.AddRange(ints);

				//add offset to each item
				for (int j = 0; j < tempInts.ToList().Count; ++j)
				{
					tempInts[j] += offset;
				}

				//if 2 words, this is ignored
				for (int i = 2; i < _wordLocationSearchers.Count; ++i)
				{
					//add offset for every new word encountered
					tempInts = _wordProximityChecker(tempInts, _wordLocationSearchers[i].ListOfIndices, _wordLocationSearchers[i].Word ?? _inputWordList[i]).ToList();
					offset += _inputWordList[i].Length;
					for (int j = 0; j < tempInts.ToList().Count; ++j)
					{
						tempInts[j] += _inputWordList[i].Length;
					}
				}

				//remove offset to give us original positions
				for (int j = 0; j < tempInts.ToList().Count; ++j)
				{
					tempInts[j] -= offset;
				}


				return tempInts;
			}

			/*
				* if not all of the searchers have their words found,
				* then search for them based on their first character
				*/


			IIndexSearch firstWordFirstCharacterLocations = new IndexSearch(_filetext, _wordLocationSearchers[0].Word?[0].ToString(), stringComparisonMethod);
			IIndexSearch secondWordFirstCharacterLocations = new IndexSearch(_filetext, _wordLocationSearchers[0].Word?[0].ToString(), stringComparisonMethod);

			//2+ word case, check the first two words
			ints = _wordProximityChecker(firstWordFirstCharacterLocations, secondWordFirstCharacterLocations);
			tempInts.AddRange(ints);

			//add offset to each item
			for (int j = 0; j < tempInts.ToList().Count; ++j)
			{
				tempInts[j] += offset;
			}

			//if 2 words, this is ignored
			for (int i = 2; i < _wordLocationSearchers.Count; ++i)
			{
				//add offset for every new word encountered
				IIndexSearch firstCharacterLocations = new IndexSearch(_filetext, (_wordLocationSearchers[i].Word ?? _inputWordList[i])[0].ToString(), stringComparisonMethod);
				tempInts = _wordProximityChecker(tempInts, _wordLocationSearchers[i].ListOfIndices, _wordLocationSearchers[i].Word ?? _inputWordList[i]).ToList();
				offset += _inputWordList[i].Length;
				for (int j = 0; j < tempInts.ToList().Count; ++j)
				{
					tempInts[j] += _inputWordList[i].Length;
				}
			}

			//remove offset to give us original positions
			for (int j = 0; j < tempInts.ToList().Count; ++j)
			{
				tempInts[j] -= offset;
			}
			return tempInts;
			

		}
		else
		{
			/*
				* this exception is intentional 
				* i want to keep the filter tight so we don't have random garbage getting in,
				* if we don't have at least the last word, we can't be 100% sure that we have the exact statistic
				*/
			StringBuilder builder = new StringBuilder();
			foreach(string t in _inputWordList) builder.Append($"{t} ");
			PFDBLogger.LogWarning($"Could not find any instances of {builder} in the text supplied.");
			throw new WordNotFoundException($"Could not find any instances of {builder} in the text supplied.");
		}
	}

	/// <summary>
	/// Filters undesirable locations that are near the desired locations. 
	/// </summary>
	/// <param name="initialLocations">The initial list of locations.</param>
	/// <returns>A filtered list without any of the unwelcome positions.</returns>
	private List<int> _removeStatisticNonGrataLocations(List<int> initialLocations)
	{
		int temp = 0;
		foreach (int u in initialLocations)
		{
			foreach (int t in _statisticNonGrataLocations)
			{
				switch (_searchTarget)
				{
					case SearchTargets.ReloadTime:
						{
							//Console.WriteLine($"empty position {t}, reload position {u}, t forward {t + "empty".Length + _acceptableSpaces}, t backward {t - "empty".Length - _acceptableSpaces}");
							//if "empty" is close enough to "reload time", we can conclude that it's not what we want and we should remove it
							if ((t < u && t + "empty".Length + _acceptableSpaces > u) || 
								(t == u) ||
								(t > u && t - "empty".Length - _acceptableSpaces < u))
							{
								temp = u;
							}
							break;
						}
					case SearchTargets.Damage:
						{
							if ((t < u && t + "damage".Length + _acceptableSpaces > u) || 
								(t == u) ||
								(t > u && t - "damage".Length - _acceptableSpaces < u))
							{
								temp = u;
							}
							break;
						}
					case SearchTargets.Suppression:
						{
							if ((t < u && t + "suppression".Length + _acceptableSpaces > u) || 
								(t == u) ||
								(t > u && t - "suppression".Length - _acceptableSpaces < u))
							{
								temp = u;
							}
							break;
						}
				}
			}
		}

		initialLocations.RemoveAll(x => x == temp && x != 0);

		return initialLocations;
	}

	/// <summary>
	/// Gets the locations of statistics we want to select against.
	/// </summary>
	/// <param name="stringComparisonMethod">Specifies the StringComparison method to be used.</param>
	/// <param name="statisticName">The name of the statistic to select against. Example: <c>["empty","reload","time"]</c> will select against "empty reload time".</param>
	private void _getStatisticNonGrataLocations(StringComparison stringComparisonMethod,params string[] statisticName)
	{
		_inputWordList.Clear();
		_wordLocationSearchers.Clear();
		_inputWordList.AddRange(_lister(statisticName));
		foreach (string word in _inputWordList)
		{
			_wordLocationSearchers.Add(new IndexSearch(_filetext, word, stringComparisonMethod));
		}
		try
		{
			_statisticNonGrataLocations = _grabStatisticLocations(stringComparisonMethod);
		}
		finally
		{
			_inputWordList.Clear();
			_wordLocationSearchers.Clear();
			_inputWordsSelection();
		}
	}

	/// <summary>
	/// Checks if one-word cases have no words at all.
	/// </summary>
	/// <param name="stringComparisonMethod">Specifies the StringComparison method to be used.</param>
	/// <exception cref="WordNotFoundException"></exception>
	private void _oneWordCaseHander(StringComparison stringComparisonMethod)
	{
		if (_wordLocationSearchers[0].IsEmpty())
		{
			_corruptedWordFixer(_inputWordList[0], stringComparisonMethod);
			_wordLocationSearchers[0].Search();
			if (_wordLocationSearchers[0].IsEmpty()) //return; //error
				throw new WordNotFoundException($"{_inputWordList[0].ToUpper()} was not found anywhere in the text with one-word case.");
		}
	}
}