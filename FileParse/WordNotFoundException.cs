using System;

namespace PFDB.Parsing;

/// <summary>
/// Defines an exception for when a word is not found.
/// </summary>
public class WordNotFoundException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="WordNotFoundException"/> class.
	/// </summary>
	public WordNotFoundException() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="WordNotFoundException"/> class with a specified error message.
	/// </summary>
	public WordNotFoundException(string message) : base(message) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="WordNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
	/// </summary>
	public WordNotFoundException(string message, Exception inner) : base(message, inner) { }

}