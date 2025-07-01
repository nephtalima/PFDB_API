using System.Collections.Generic;
using PFDB.StatisticUtility;

namespace PFDB.StatisticStructure;


/// <summary>
/// Interface that defines a collection of statistics (i.e. <see cref="IStatistic"/>).
/// </summary>
public interface IStatisticCollection : IPFDBCollection
{
	/// <summary>
	/// The exposed collection of statistics.
	/// </summary>
	public IEnumerable<IStatistic> Statistics { get; }

	/// <summary>
	/// Indicates the statistics that should be in the collection, but are not present. Has elements if <see cref="IsMissingStatistic"/> is true.
	/// </summary>
	public IEnumerable<StatisticOptions> MissingStatistics { get; }

	/// <summary>
	/// Indicates if the collection is missing a statistic. If true, there will be an element within <see cref="MissingStatistics"/>.
	/// </summary>
	public bool IsMissingStatistic { get; }

	/// <summary>
	/// Adds a statistic to the collection.
	/// </summary>
	/// <param name="statistic">The statistic to add.</param>
	public void Add(IStatistic statistic);


	/// <summary>
	/// Adds a collection of statistics to the collection.
	/// </summary>
	/// <param name="statistics">The statistic to add.</param>
	public void AddRange(IEnumerable<IStatistic> statistics);

	// This method conflicts with AddRange(IEnumerable<IStatistic>) because
	// the compiler cannot figure out which call to use :(
	//void AddRange(IStatisticCollection collection);
}
