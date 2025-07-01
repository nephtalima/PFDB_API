using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.ParsingUtility;
/// <summary>
/// Defines a utility class for parsing and searching.
/// </summary>
public static class ParsingUtilityClass
{
	/// <summary>
	/// Gets a list of search targets for a given weapon type.
	/// </summary>
	/// <param name="weaponType">Indicates the weapon type to get the appropriate search targets.</param>
	/// <returns>A list of search targets for the specified weapon type.</returns>
	public static IEnumerable<SearchTargets> GetSearchTargetsForWeapon(WeaponType weaponType)
	{
		List<SearchTargets> searchTargets = new List<SearchTargets>();

		switch(weaponType)
		{
			case WeaponType.Grenade:
				{
					searchTargets.AddRange(Enum.GetValues<SearchTargets>().ToList().Where(x => x >= SearchTargets.BlastRadius && x <= SearchTargets.StoredCapacity));
					break;
				}
			case WeaponType.Melee:
				{
					searchTargets.AddRange(Enum.GetValues<SearchTargets>().ToList().Where(x => x >= SearchTargets.FrontStabDamage && x <= SearchTargets.Walkspeed));
					searchTargets.AddRange(Enum.GetValues<SearchTargets>().ToList().Where(x => x >= SearchTargets.HeadMultiplier && x <= SearchTargets.LimbMultiplier));

					break;
				}
			//default case is a gun
			default:
				{
					searchTargets.AddRange(Enum.GetValues<SearchTargets>().ToList().Where(x => x >= SearchTargets.Rank && x <= SearchTargets.FireModes));
					break;
				}
		}

		return searchTargets;
	}
}
