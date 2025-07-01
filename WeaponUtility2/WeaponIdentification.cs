using PFDB.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.WeaponUtility;


/// <summary>
/// Provides a unique identifier for every single weapon across every single version.
/// Useful for determining whether two weapons from the same version are the same.
/// Keep in mind that weapons that are the same rank and name across versions will not be the same here.
/// It is recommended to cross-reference this data with the weapon database to accurately check.
/// </summary>
public class WeaponIdentification
{
	private const short _dummyDigit = 1;
	//private short _checksum = 0; //im still not sure if i will implement this
	private long _underlyingIntegerCode;
	private readonly PhantomForcesVersion _version;
	private readonly Categories _category;
	private readonly int _rank;
	private readonly int _rankTieBreaker;
	private readonly string _weaponName;

	private const int _totalVersionSpace = 4;
	private const int _totalCategorySpace = 2;
	private const int _totalRankSpace = 7;
	private const int _totalRankTieBreakerSpace = 2;

	/// <summary>
	/// The underlying number that uniquely identifies a weapon.
	/// </summary>
	public long ID { get { return _underlyingIntegerCode; } }

	/// <summary>
	/// The version that the weapon belongs to.
	/// </summary>
	public PhantomForcesVersion Version { get { return _version; } }

	/// <summary>
	/// The rank of the weapon. Maximum rank is 9999999 (7 digits)
	/// </summary>
	public int Rank { get { return _rank; } }

	/// <summary>
	/// If two weapons are the same rank, this number determines the sort order according to the order of the screenshots provided. 
	/// This value mostly comes from the weapon database.
	/// Maximum tiebreaker is 99 (2 digits)
	/// </summary>
	public int RankTieBreaker { get { return _rankTieBreaker; } }

	/// <summary>
	/// The name of the weapon.
	/// </summary>
	public string WeaponName { get { return _weaponName; } }

	/// <summary>
	/// Identifies if the weapon is a primary, secondary, melee or grenade.
	/// </summary>
	public WeaponType WeaponType { get { return WeaponUtilityClass.GetWeaponType(_category); } }

	/// <summary>
	/// Identifies what category the weapon belongs to.
	/// </summary>
	public Categories Category { get { return _category; } }

	/// <summary>
	/// Constructor for populating all of the fields when only the underlying number is known. Used by the weapon database.
	/// </summary>
	/// <param name="integerIdentification">The underlying unique identifier of the number.</param>
	/// <param name="weaponName">Optional weapon name.</param>
	public WeaponIdentification(long integerIdentification, string weaponName = "")
	{
		ReadOnlySpan<char> idcode = integerIdentification.ToString();
		const int dummySpace = 1;
		ReadOnlySpan<char> version = idcode.Slice(dummySpace, _totalVersionSpace);
		ReadOnlySpan<char> category = idcode.Slice(dummySpace + _totalVersionSpace, _totalCategorySpace);
		ReadOnlySpan<char> rank = idcode.Slice(dummySpace + _totalVersionSpace + _totalCategorySpace, _totalRankSpace);
		ReadOnlySpan<char> rankTieBreaker = idcode.Slice(dummySpace + _totalCategorySpace + _totalVersionSpace + _totalRankSpace, _totalRankTieBreakerSpace);

		_version = new PhantomForcesVersion(version.ToString());
		_underlyingIntegerCode = long.Parse(idcode);
		int tempcategory = int.Parse(category);
		if (tempcategory < 0 || tempcategory > (int)Categories.TwoHandBluntMelees) PFDBLogger.LogError("Error reading parameter. The category specified is invalid", parameter: _underlyingIntegerCode);
		_category = (Categories)tempcategory;
		_rank = int.Parse(rank);
		_rankTieBreaker = int.Parse(rankTieBreaker);

		_weaponName = weaponName;
	}

	/// <summary>
	/// Default constructor for when the following parameters are known. Populates all fields of the object.
	/// </summary>
	/// <param name="version">The version of Phantom Forces that the weapon belongs to.</param>
	/// <param name="category">The category of the weapon.</param>
	/// <param name="rank">The rank of the weapon. Must not exceed 10 001 (as no weapon exists with that rank)</param>
	/// <param name="rankTieBreaker">The rank tiebreaker number. Must not exceed 99 and must be unique to the weapon if there exists another weapon with the same rank.</param>
	/// <param name="weaponName">The name of the weapon. Optional.</param>
	/// <exception cref="ArgumentException"></exception>
	public WeaponIdentification(PhantomForcesVersion version, Categories category, int rank, int rankTieBreaker, string weaponName = "")
	{
		if (rank > 10001)
		{
			PFDBLogger.LogFatal("Cannot instantiate WeaponIdentification with a rank number greater than 10 001.", parameter: rank);
			throw new ArgumentException("Cannot instantiate WeaponIdentification with a rank number greater than 10 001."); //invalid
		}
		if (rankTieBreaker > 99)
		{
			PFDBLogger.LogFatal("Cannot instantiate WeaponIdentification with a rank tiebreaker number greater than 99.", parameter: rankTieBreaker);
			throw new ArgumentException("Cannot instantiate WeaponIdentification with a rank tiebreaker number greater than 99."); //invalid
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(_dummyDigit);

		_version = version;
		_category = category;
		_rank = rank;
		_rankTieBreaker = rankTieBreaker;
		_weaponName = weaponName;

		int versionPadding = _totalVersionSpace - _version.VersionNumber.ToString().Length;
		stringBuilder.Append('0', versionPadding);
		stringBuilder.Append(_version.VersionNumber);

		int categoryPadding = _totalCategorySpace - ((int)_category).ToString().Length;
		stringBuilder.Append('0', categoryPadding);
		stringBuilder.Append((int)_category);

		int rankPadding = _totalRankSpace - _rank.ToString().Length;
		stringBuilder.Append('0', rankPadding);
		stringBuilder.Append(_rank);

		int rankTieBreakerPadding = _totalRankTieBreakerSpace - _rankTieBreaker.ToString().Length;
		stringBuilder.Append('0', rankTieBreakerPadding);
		stringBuilder.Append(_rankTieBreaker);

		_underlyingIntegerCode = Convert.ToInt64(stringBuilder.ToString());
	}

	/// <summary>
	/// Overloaded equality comparator.
	/// </summary>
	/// <param name="first">The first object.</param>
	/// <param name="second">The second object.</param>
	/// <returns>True if both objects are equal, false otherwise.</returns>
	public static bool operator ==(WeaponIdentification first, WeaponIdentification second)
	{
		return first.ID == second.ID;
	}

	/// <summary>
	/// Overloadedin equality comparator.
	/// </summary>
	/// <param name="first">The first object.</param>
	/// <param name="second">The second object.</param>
	/// <returns>True if both objects are inequal, false otherwise.</returns>
	public static bool operator !=(WeaponIdentification first, WeaponIdentification second)
	{
		return first.ID != second.ID;
	}

	/// <summary>
	/// Overriden default eqaulity comparer.
	/// </summary>
	/// <param name="obj">The object to compare with.</param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(obj, null))
		{
			return false;
		}
		if (ReferenceEquals(this, obj))
		{
			return true;
		}


		throw new Exception();
	}

	/// <summary>
	/// Gets the hash code of the object.
	/// </summary>
	/// <returns>The hash code of the current object.</returns>
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
