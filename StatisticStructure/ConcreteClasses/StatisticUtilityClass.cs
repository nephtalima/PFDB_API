using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PFDB.StatisticUtility;


/// <summary>
/// Defines a utility class for statistics.
/// </summary>
public static class StatisticUtilityClass
{
	/// <summary>
	/// Gets a list of statistic options for a given weapon type.
	/// </summary>
	/// <param name="weaponType">Indicates the weapon type to get the appropriate statistic options.</param>
	/// <returns>A list of statistic options for the specified weapon type.</returns>
	public static IEnumerable<StatisticOptions> GetSearchTargetsForWeapon(WeaponType weaponType)
	{
		List<StatisticOptions> searchTargets = new List<StatisticOptions>();

		switch (weaponType)
		{
			case WeaponType.Grenade:
				{
					searchTargets.AddRange(Enum.GetValues<StatisticOptions>().ToList().Where(x => x >= StatisticOptions.BlastRadius && x <= StatisticOptions.StoredCapacity));
					break;
				}
			case WeaponType.Melee:
				{
					searchTargets.AddRange(Enum.GetValues<StatisticOptions>().ToList().Where(x => x >= StatisticOptions.FrontStabDamage && x <= StatisticOptions.Walkspeed));
					searchTargets.AddRange(Enum.GetValues<StatisticOptions>().ToList().Where(x => x >= StatisticOptions.HeadMultiplier && x <= StatisticOptions.LimbMultiplier));

					break;
				}
			default:
				{
					searchTargets.AddRange(Enum.GetValues<StatisticOptions>().ToList().Where(x => x >= StatisticOptions.Rank && x <= StatisticOptions.FireModes));
					break;
				}
		}

		return searchTargets;
	}
}
