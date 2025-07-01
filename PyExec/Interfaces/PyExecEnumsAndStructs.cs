namespace PFDB.PythonExecutionUtility
{
	/// <summary>
	/// Specifies the output destination. Use <c><see cref="OutputDestination.Console"/> | <see cref="OutputDestination.File"/></c> to specify both outputs.
	/// </summary>
	public enum OutputDestination
	{
		/// <summary>
		/// Output to the console.
		/// </summary>
		Console = 1,
		/// <summary>
		/// Output to a file. The file is generated in <code>{currentWorkingDirectory}/</code>
		/// </summary>
		File
	}
}


namespace PFDB.PythonFactoryUtility
{
	/// <summary>
	/// Status counter for work items.
	/// </summary>
	public struct StatusCounter
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="SuccessCounter">Counter for successes.</param>
		/// <param name="FailCounter">Counter for failures.</param>
		public StatusCounter(int SuccessCounter, int FailCounter)
		{
			this.SuccessCounter = SuccessCounter;
			this.FailCounter = FailCounter;
		}

		/// <summary>
		/// The number of successes.
		/// </summary>
		public int SuccessCounter;
		/// <summary>
		/// The number of failures.
		/// </summary>
		public int FailCounter;
	}


	/// <summary>
	/// The number of logical processors the central processing unit (CPU) has.
	/// </summary>
	public enum Cores
	{
		/// <summary>
		/// Single core.
		/// </summary>
		Single = 1,
		/// <summary>
		/// Dual core.
		/// </summary>
		Dual,
		/// <summary>
		/// Quadruple core.
		/// </summary>
		Four = 4,
		/// <summary>
		/// Sextuple core.
		/// </summary>
		Six = 6,
		/// <summary>
		/// Octuple core.
		/// </summary>
		Eight = 8,
		/// <summary>
		/// Decuple core.
		/// </summary>
		Ten = 10,
		/// <summary>
		/// Duodecuple core.
		/// </summary>
		Twelve = 12,
		/// <summary>
		/// Quattordecuple core.
		/// </summary>
		Fourteen = 14,
		/// <summary>
		/// Sexdecuple core.
		/// </summary>
		Sixteen = 16,
		/// <summary>
		/// Octodecuple core.
		/// </summary>
		Eighteen = 18,
		/// <summary>
		/// Vigintuple core.
		/// </summary>
		Twenty = 20
	}
}
