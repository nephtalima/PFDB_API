namespace PFDB.WeaponUtility;


/// <summary>
/// Defines an interface for a weapon class.
/// </summary>
public interface IClass
{
	/// <summary>
	/// If the current category requries manual proofreading.
	/// </summary>
	bool NeedsRevision { get; }
	/// <summary>
	/// Defines the type of the weapon class.
	/// </summary>
	Classes ClassType { get; }
	/// <summary>
	/// Defines the underlying collection of categories.
	/// </summary>
	ICategoryCollection CategoryCollection { get; }
}
