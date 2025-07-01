using PFDB.StatisticUtility;
using PFDB.WeaponUtility;
using System.Collections.Generic;
using System.Linq;

namespace PFDB.StatisticStructure;


/// <summary>
/// Defines a statistic.
/// </summary>
public class Statistic : IStatistic
{
	private bool _needsRevision;
	private IEnumerable<string> _statistics;
	private WeaponIdentification _WID;
	private StatisticOptions _option;

	/// <summary>
	/// List of strings for a given statistic. Assume only one entry, except for <see cref="StatisticOptions.FireModes"/> and <see cref="StatisticOptions.Firerate"/> for certain weapons.
	/// </summary>
	public IEnumerable<string> Statistics { get { return _statistics; } }
	/// <inheritdoc/>
	public bool NeedsRevision { get { return _needsRevision; } }
	/// <inheritdoc/>
	public WeaponIdentification WeaponID { get { return _WID; } }
	/// <inheritdoc/>
	public StatisticOptions Option { get { return _option; } }
	/// <inheritdoc/>
	public WeaponType WeaponType { get { return _WID.WeaponType; ; } }

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="needsRevision">Indicates if the statistic is faulty and needs revision.</param>
	/// <param name="statistics">The list of statistics to group under this statistic object.</param>
	/// <param name="weaponID">The weapon ID of the statistics.</param>
	/// <param name="option">The type of statistic being added.</param>
	public Statistic(bool needsRevision, IEnumerable<string> statistics, WeaponIdentification weaponID, StatisticOptions option)
	{
		_WID = weaponID;
		_option = option;
		_statistics = statistics;
		_needsRevision = needsRevision;

		if (needsRevision == false && statistics.Any() == false)
			_needsRevision = true;
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="needsRevision">Indicates if the statistic is faulty and needs revision.</param>
	/// <param name="statistic">The underlying statistic for this statistic object.</param>
	/// <param name="weaponID">The weapon ID of the statistics.</param>
	/// <param name="option">The type of statistic being added.</param>
	public Statistic(bool needsRevision, string statistic, WeaponIdentification weaponID, StatisticOptions option)
	{
		_WID = weaponID;
		_option = option;
		_statistics = new List<string>() { statistic };
		_needsRevision = needsRevision;

		if (needsRevision == false && statistic == string.Empty)
			_needsRevision = true;
	}

}