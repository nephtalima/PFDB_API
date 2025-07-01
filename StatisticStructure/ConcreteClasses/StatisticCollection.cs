using PFDB.Logging;
using PFDB.StatisticUtility;
using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.StatisticStructure;


/// <summary>
/// Defines a collection of statistics (i.e. <see cref="IStatistic"/>). Inherits from <see cref="List{IStatistic}"/> with additional fields
/// </summary>
public class StatisticCollection : List<IStatistic>, IStatisticCollection
{
	/// <inheritdoc/>
	public bool CollectionNeedsRevision { get {
			return this.Any(x => x.NeedsRevision);
		} }

	private WeaponIdentification _WID;
	private IEnumerable<StatisticOptions> _missingStatistics = new List<StatisticOptions>();

	/// <inheritdoc/>
	public WeaponIdentification WeaponID { get { return _WID; } }

	/// <inheritdoc/>
	public IEnumerable<StatisticOptions> MissingStatistics { get { return _missingStatistics; } }

	/// <inheritdoc/>
	public bool IsMissingStatistic { get
		{
			return _missingStatistics.Count() > 0;
		} }

	/// <inheritdoc/>
	public IEnumerable<IStatistic> Statistics => this;

	/// <inheritdoc/>
	public new void Add(IStatistic statistic)
	{
		if (statistic.WeaponID != _WID)
		{
			PFDBLogger.LogError($"Cannot add IStatistic object to StatisticCollection. Version numbers are invalid. StatisticCollection version: {_WID.Version.VersionString}, IStatistic version: {statistic.WeaponID.Version.VersionString}", parameter: statistic);
			throw new ArgumentException($"Cannot add IStatistic object to StatisticCollection. Version numbers are invalid. StatisticCollection version: {_WID.Version.VersionString}, IStatistic version: {statistic.WeaponID.Version.VersionString}", nameof(statistic));
		}

		base.Add(statistic);
	}

	/// <inheritdoc/>
	public new void AddRange(IEnumerable<IStatistic> statistics) {
		if (statistics.Any(x => x.WeaponID != this._WID))
		{
			PFDBLogger.LogError($"Cannot add IStatistic objects to StatisticCollection. Version numbers are invalid. StatisticCollection version: {_WID.Version.VersionString}, IStatistic version: {statistics.First().WeaponID.Version.VersionString}");
			throw new ArgumentException($"Cannot add IStatistic object to StatisticCollection. Version numbers are invalid. StatisticCollection version: {_WID.Version.VersionString}, IStatistic version: {statistics.First().WeaponID.Version.VersionString}");
		}
		base.AddRange(statistics);
	}

	/*
	public void AddRange(IStatisticCollection statistics)
	{
		if(statistics.WeaponID.Version != this._WID.Version)
		{
			PFDBLogger.LogError($"Cannot add specified StatisticCollection to current StatisticCollection. Version numbers are invalid. Current StatisticCollection version: {_WID.Version.VersionString}, Specified StatisticCollection version: {statistics.WeaponID.Version.VersionString}");
			throw new ArgumentException($"Cannot add specified StatisticCollection object to current StatisticCollection. Version numbers are invalid. Current StatisticCollection version: {_WID.Version.VersionString}, Specified StatisticCollection version: {statistics.WeaponID.Version.VersionString}");
		}

		base.AddRange(statistics.Statistics);
	}*/

	/// <summary>
	/// Initializes an empty collection with a default size of 0.
	/// </summary>
	/// <param name="weaponID">The unique weapon identifier for the collection.</param>
	public StatisticCollection(WeaponIdentification weaponID) : base() {
		_WID = weaponID;
	}

	/// <summary>
	/// Initializes an empty collection with a specified size.
	/// </summary>
	/// <param name="weaponID">The unique weapon identifier for the collection.</param>
	/// <param name="capacity">The specified size of the collection.</param>
	public StatisticCollection(WeaponIdentification weaponID, int capacity) : base(capacity) {
		_WID = weaponID;
	}

	/// <summary>
	/// Initializes a collection with items from a specified collection.
	/// </summary>
	/// <param name="weaponID">The unique weapon identifier for the collection.</param>
	/// <param name="collection">The initial collection to initialize for the the current collection.</param>
	public StatisticCollection(WeaponIdentification weaponID, IEnumerable<IStatistic> collection) : base(collection) {
		_WID = weaponID;
	}

}