// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;

namespace PFDB.Parsing;

/// <summary>
/// Defines an interface that searches for indexes of a word inside text.
/// </summary>
internal interface IIndexSearch
{
	/// <summary>
	/// The list of indices specifying the locations of the word.
	/// </summary>
	public List<int> ListOfIndices { get; }

	/// <summary>
	/// The string comparison method for searching.
	/// </summary>
	public StringComparison StringComparisonMethod { get; }

	/// <summary>
	/// The text to search.
	/// </summary>
	public string Text { get; }

	/// <summary>
	/// The word to search inside the text.
	/// </summary>
	public string? Word { get; }

	/// <summary>
	/// Determines if <see cref="ListOfIndices"/> is empty.
	/// </summary>
	/// <returns>True if <see cref="ListOfIndices"/> is empty, false otherwise.</returns>
	public bool IsEmpty();

	/// <summary>
	/// Removes a list of locations from the underlying list.
	/// </summary>
	/// <param name="list">The list to remove.</param>
	public void RemoveFromList(IEnumerable<int> list);

	/// <summary>
	/// Searches the text for the word. 
	/// </summary>
	/// <returns>A collection of non-negative integers indicating where the word is.</returns>
	public IEnumerable<int> Search();
}
