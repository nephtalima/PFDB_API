using PFDB.Logging;
using System;
using System.Text.RegularExpressions;

namespace PFDB.WeaponUtility;



/// <summary>
/// Specifies the weapon type.
/// </summary>
public enum WeaponType
{
	/// <summary>
	/// Primary Gun.
	/// </summary>
	Primary = 1,
	/// <summary>
	/// Secondary Gun.
	/// </summary>
	Secondary,
	/// <summary>
	/// Grenade.
	/// </summary>
	Grenade,
	/// <summary>
	/// Melee.
	/// </summary>
	Melee
}


/// <summary>
/// Specifies the version of Phantom Forces.
/// </summary>
public sealed class PhantomForcesVersion : IComparable<PhantomForcesVersion>
{
	private string _versionString;

	/// <summary>
	/// User-friendly version number.
	/// </summary>
	public string VersionString
	{
		get { return _versionString; }
	}

	/// <summary>
	/// Integer version of <see cref="VersionString"/>.
	/// </summary>
	public int VersionNumber
	{
		get
		{
			string tempstr = _versionString;
			while (tempstr.Contains('.'))
			{
				try
				{
					tempstr = tempstr.Remove(tempstr.LastIndexOf('.'), 1);
				}
				catch (ArgumentOutOfRangeException)
				{
					break;
				}
			}
			return Convert.ToInt32(tempstr);
		}
	}

	/// <summary>
	/// Returns true if the version is earlier than <c>9.0.0</c>.
	/// </summary>
	public bool IsLegacy
	{
		get
		{
			return VersionNumber < 900;
		}
	}

	/// <summary>
	/// Determines the number of screenshots that were taken for the given version.
	/// </summary>
	/// <returns>The number of screenshots for the given version</returns>
	public int MultipleScreenshotsCheck()
	{
		if (IsLegacy)
		{
			return 2;
		} else if (VersionNumber > 1012)
		{
			return 2;
		}
		else
		{
			return 1;
		}
	}

	/// <summary>
	/// Equality operator.
	/// </summary>
	/// <param name="first">First object to compare.</param>
	/// <param name="second">Second object to compare.</param>
	/// <returns>True if the versions are equal, false if unequal.</returns>
	public static bool operator ==(PhantomForcesVersion first, PhantomForcesVersion second)
	{
		return first.VersionNumber == second.VersionNumber;
	}

	/// <summary>
	/// Non-equality operator.
	/// </summary>
	/// <param name="first">First object to compare.</param>
	/// <param name="second">Second object to compare.</param>
	/// <returns>True if the versions are unequal, false if equal.</returns>
	public static bool operator !=(PhantomForcesVersion first, PhantomForcesVersion second)
	{
		return first.VersionNumber != second.VersionNumber;
	}

	/// <summary>
	/// Greater-than operator.
	/// </summary>
	/// <param name="first">First object to compare.</param>
	/// <param name="second">Second object to compare.</param>
	/// <returns>True if the first object is greater than the second object.</returns>
	public static bool operator >(PhantomForcesVersion first, PhantomForcesVersion second)
	{
		return first.VersionNumber > second.VersionNumber;
	}

	/// <summary>
	/// Less-than operator.
	/// </summary>
	/// <param name="first">First object to compare.</param>
	/// <param name="second">Second object to compare.</param>
	/// <returns>True if the first object is less than the second object.</returns>
	public static bool operator <(PhantomForcesVersion first, PhantomForcesVersion second)
	{
		return first.VersionNumber < second.VersionNumber;
	}

	/// <summary>
	/// Greater-than-or-equal-to operator.
	/// </summary>
	/// <param name="first">First object to compare.</param>
	/// <param name="second">Second object to compare.</param>
	/// <returns>True if the first object is greater than or equal to the second object.</returns>
	public static bool operator >=(PhantomForcesVersion first, PhantomForcesVersion second)
	{
		return first.VersionNumber >= second.VersionNumber;
	}

	/// <summary>
	/// Less-than-or-equal-to operator.
	/// </summary>
	/// <param name="first">First object to compare.</param>
	/// <param name="second">Second object to compare.</param>
	/// <returns>True if the first object is less than or equal to the second object.</returns>
	public static bool operator <=(PhantomForcesVersion first, PhantomForcesVersion second)
	{
		return first.VersionNumber <= second.VersionNumber;
	}


	/// <summary>
	/// Constructor using major, minor and revision versions of Phantom Forces.
	/// For example, in the version "8.0.1":
	/// <list type="bullet">
	/// <item>8 = Major version</item>
	/// <item>0 = Minor version</item>
	/// <item>1 = Revision</item>
	/// </list>
	/// </summary>
	/// <param name="majorVersion">Major version of Phantom Forces.</param>
	/// <param name="minorVersion">Minor version of Phantom Forces.</param>
	/// <param name="revision">Patch/Revision version of Phantom Forces</param>
	public PhantomForcesVersion(int majorVersion, int minorVersion, int revision)
	{
		_versionString = $"{majorVersion}.{minorVersion}.{revision}";
	}

	/// <summary>
	/// Constructor using the in-game version number.
	/// </summary>
	/// <param name="versionString">The in-game version number. For example: "8.0.1g" or "8.0.1" will both work for this parameter.</param>
	public PhantomForcesVersion(string versionString)
	{
		//alternate: ^(\d+\.?\d+\.?\d+)(\D+)
		//matches "1010" as well, but can also match "10.10" :C

		Regex regexpart1 = new Regex(@"^(\d+\.\d+\.\d+)(\D+)$");
		Match matches = regexpart1.Match(versionString);
		if (matches.Groups.Count > 1)
		{
			_versionString = matches.Groups[1].Value;
		}
		else
		{
			ReadOnlySpan<char> span = versionString;
			if (span.Length == 3)
			{
				int majorVersion = int.Parse(span[..1]);
				int minorVersion = int.Parse(span.Slice(1, 1));
				int revision = int.Parse(span.Slice(2, 1));
				_versionString = $"{majorVersion}.{minorVersion}.{revision}";
			}
			else if (span.Length == 4)
			{
				int majorVersion = int.Parse(span[..2]);
				int minorVersion = int.Parse(span.Slice(2, 1));
				int revision = int.Parse(span.Slice(3, 1));
				_versionString = $"{majorVersion}.{minorVersion}.{revision}";
			} else if (span.Contains('.'))
			{
				_versionString = versionString;
			}
			else
			{
				throw new ArgumentException("invalid version");
			}
		}
	}

	/// <summary>
	/// Determines whether the specified object equals the current object.
	/// </summary>
	/// <param name="obj">The specified object to compare.</param>
	/// <returns>Whether the specified objest is equal to the current object.</returns>
	/// <exception cref="NotImplementedException"></exception>
	public override bool Equals(object? obj)
	{
		if (obj != null)
		{
			if (obj is PhantomForcesVersion objc)
			{
				return objc.VersionNumber == this.VersionNumber;
			}
		}
		return false;
	}

	/// <summary>
	/// Hash function.
	/// </summary>
	/// <returns>The default hash return.</returns>
	public override int GetHashCode()
	{
		return VersionNumber;
	}

	/// <inheritdoc/>
	public int CompareTo(PhantomForcesVersion? other)
	{
		if (other is null) throw new ArgumentNullException("Object being compared to cannot be null");

		if (other < this) return 1;
		if (other == this) return 0;
		if (other > this) return -1;

		throw new Exception("uh what");
	}
}


/// <summary>
/// A dedicated utility class for weapon types in Phantom Forces.
/// </summary>
public static class WeaponUtilityClass
{
	/// <summary>
	/// Converts an integer to the corresponding <see cref="WeaponType"/> category.
	/// </summary>
	/// <param name="categoryNumber">The category number. This value cannot exceed 19.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	public static WeaponType GetWeaponType(int categoryNumber)
	{
		if (categoryNumber > 19)
		{
			PFDBLogger.LogWarning("The category number cannot exceed 19.");
			throw new ArgumentException("The category number cannot exceed 19.", nameof(categoryNumber));
		}
		if (categoryNumber < 0)
		{
			PFDBLogger.LogWarning("The category number cannot be negative.");
			throw new ArgumentException("The category number cannot be negative.", nameof(categoryNumber));
		}
		WeaponType weaponType = 0;
		if (categoryNumber < 8)
		{
			weaponType = WeaponType.Primary;
		}
		else if (categoryNumber >= 8 && categoryNumber < 12)
		{
			weaponType = WeaponType.Secondary;
		}
		else if (categoryNumber >= 12 && categoryNumber < 15)
		{
			weaponType = WeaponType.Grenade;
		}
		else
		{
			weaponType = WeaponType.Melee;
		}
		return weaponType;
	}

	/// <summary>
	/// Gets the category type of the supplied category number.
	/// </summary>
	/// <param name="categoryNumber">A number that ranges from 0 to 18 inclusively.</param>
	/// <returns>The category type of the specified number.</returns>
	/// <exception cref="ArgumentException"></exception>
	public static Categories GetCategoryType(int categoryNumber)
	{
		if (categoryNumber > 18)
		{
			PFDBLogger.LogWarning("The category number cannot exceed 19");
			throw new ArgumentException("The category number cannot exceed 19.", nameof(categoryNumber));
		}
		if (categoryNumber < 0)
		{

			PFDBLogger.LogWarning("The category number cannot be negative");
			throw new ArgumentException("The category number cannot be negative.", nameof(categoryNumber));
		}
		return (Categories)categoryNumber;
	}

	/// <summary>
	/// Gets the weapon type given a category type.
	/// </summary>
	/// <param name="category">The category type.</param>
	/// <returns>The weapon type that the category belongs to.</returns>
	public static WeaponType GetWeaponType(Categories category)
	{
		return GetWeaponType((int)category);
	}
}
