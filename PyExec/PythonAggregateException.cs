using System;
using System.Collections.Generic;

namespace PFDB;


/// <summary>
/// Used for throwing multiple exceptions that occured during Python execution
/// </summary>
public sealed class PythonAggregateException : SystemException
{
	/// <summary>
	/// The error message.
	/// </summary>
	public override string Message { get; }

	internal List<SystemException> exceptions { get; set; }

	/// <summary>
	/// Default constructor.
	/// </summary>
	public PythonAggregateException()
	{
		Message = "Multiple errors occured";
		exceptions = new List<SystemException>();
	}

	/// <summary>
	/// Default constructor, with message.
	/// </summary>
	/// <param name="message">The error message.</param>
	public PythonAggregateException(string message) : base(message)
	{
		exceptions = new List<SystemException>();
		Message = message;
	}

	/// <summary>
	/// Default constructor, with message and inner exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="innerException">The encapsulated exception.</param>
	public PythonAggregateException(string message, Exception innerException) : base(message, innerException)
	{
		exceptions = new List<SystemException>();
		Message = message;
	}

}

/*
[global::System.Serializable]
public class MyException : global::System.Exception
{
	public MyException() { }
	public MyException(string message) : base(message) { }
	public MyException(string message, global::System.Exception inner) : base(message, inner) { }
	protected MyException(
	  global::System.Runtime.Serialization.SerializationInfo info,
	  global::System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
*/