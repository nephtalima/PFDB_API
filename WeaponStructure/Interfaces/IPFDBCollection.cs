namespace PFDB.WeaponUtility;


/// <summary>
/// Defines a collection that adds a check for manual proofreading.
/// </summary>
public interface IPFDBCollection
{
	/// <summary>
	/// Whether a collection requires manual proofreading.
	/// </summary>
	public bool CollectionNeedsRevision { get; }
}
