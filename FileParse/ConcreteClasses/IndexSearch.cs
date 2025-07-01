using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PFDB.Parsing;


/// <summary>
/// Defines a class that searches for indexes of a word inside text.
/// </summary>
public sealed class IndexSearch : IIndexSearch
{
	/// <inheritdoc/>
	public string Text { get; init; }

	/// <inheritdoc/>
	public string? Word { get; init; }

	/// <inheritdoc/>
	public StringComparison StringComparisonMethod { get; init; }

	/// <inheritdoc/>
	public List<int> ListOfIndices { get; private set; }


	/// <summary>
	/// Default constructor.
	/// </summary>
	/// <param name="text">Text to search within.</param>
	/// <param name="word">Word to search for.</param>
	/// <param name="searchAutomatically">Specifies whether to search immediately after creation. True by default.</param>
	public IndexSearch(string text, string? word, bool searchAutomatically = true)
	{
		ListOfIndices = new List<int>();
		Text = text;
		Word = word;
		StringComparisonMethod = StringComparison.InvariantCultureIgnoreCase;
		if (searchAutomatically) Search(); //automatically searches
	}

	/// <summary>
	/// Default constructor.
	/// </summary>
	/// <param name="text">Text to search within.</param>
	/// <param name="word">Word to search for.</param>
	/// <param name="stringComparisonMethod">Specifies the StringComparison method to be used.</param>
	/// <param name="searchAutomatically">Specifies whether to search immediately after creation. True by default.</param>
	public IndexSearch(string text, string? word, StringComparison stringComparisonMethod, bool searchAutomatically = true)
	{
		ListOfIndices = new List<int>();
		Text = text;
		Word = word;
		StringComparisonMethod = stringComparisonMethod;
		if (searchAutomatically) Search(); //automatically searches
	}

	/// <inheritdoc/>
	public bool IsEmpty()
	{
		return ListOfIndices.Count == 0;
	}

	/// <inheritdoc/>
	public void RemoveFromList(IEnumerable<int> list)
	{
		ListOfIndices = ListOfIndices.Except(list).ToList();

	}

	/// <inheritdoc/>
	public IEnumerable<int> Search()
	{
		int startIndex = 0;
		try
		{
			//annoying visual studio green squiggly
#pragma warning disable CS8604
			startIndex = Text.LastIndexOf(Environment.GetEnvironmentVariable("pythonSignalText"));
#pragma warning restore CS8604
		}
		catch { }
		//if not found, skip (it don't matter)

		//prevent negative startindex
		if (startIndex < 0 || startIndex > Text.Length - 1)
		{
			startIndex = 0;
		}

		if (Word == null) return ListOfIndices; //early return if null


		string _filetext = Text.Substring(startIndex);
		bool _isChar = Word.Length == 1;

		for (; //if we are searching for a single character, use char overload for String.Contains()
			_isChar ? _filetext.Contains(Word[0], StringComparisonMethod) : _filetext.Contains(Word, StringComparisonMethod);
			)
		{
			//try adding the found index. if lastfoundindex returns -1 (not found), stop the loop
			try
			{

				ListOfIndices.Add(
					_isChar ? _filetext.LastIndexOf(Word[0]) : _filetext.LastIndexOf(Word, StringComparisonMethod)
					);
				_filetext = _isChar
					? _filetext.Remove(_filetext.LastIndexOf(Word[0])) 
					: _filetext.Remove(_filetext.LastIndexOf(Word, StringComparisonMethod), Word.Length);
			}
			catch (ArgumentOutOfRangeException) { break; }
		}
		//clean up and make sure there isn't a -1 in the list, because we don't care about it and don't need it
		ListOfIndices.RemoveAll(x => x == -1);
		return ListOfIndices;
	}
}