using System;
using System.Collections.Generic;
using PFDB.ParsingUtility;
using PFDB.WeaponUtility;

namespace PFDB.Parsing;

/// <summary>
/// Defines an interface that parses results out from a file.
/// </summary>
internal interface IStatisticParse
{
	string Filetext { get; }
	
	/// <summary>
	/// The unique weapon identifier for the file.
	/// </summary>
	WeaponIdentification WeaponID { get; }

	/// <summary>
	/// Finds a certain statistic (specified by <c>target</c>).
	/// Stops reading the statistic when it encounters any of the endings specified by <c>endings</c>.
	/// </summary>
	/// <param name="target">Indicates which statistic to search for.</param>
	/// <param name="endings">Indicates the endings at which the extraction will stop.</param>
	/// <param name="stringComparisonMethod">Specifies the StringComparison method to be used.</param>
	/// <returns></returns>
	string FindStatisticInFile(SearchTargets target, IEnumerable<char> endings, StringComparison stringComparisonMethod);
}
