namespace PFDB.WeaponUtility;


/// <summary>
/// Defines a interface for categories.
/// </summary>
public interface ICategory
{
	/// <summary>
	/// If the current category requries manual proofreading.
	/// </summary>
	bool NeedsRevision { get; }
	/// <summary>
	/// Defines the category type.
	/// </summary>
	Categories CategoryType { get; }
	/// <summary>
	/// The underlying collection of weapons.
	/// </summary>
	IWeaponCollection WeaponCollection { get; }
}

