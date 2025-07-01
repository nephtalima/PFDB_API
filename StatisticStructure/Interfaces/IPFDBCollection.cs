using PFDB.WeaponUtility;

namespace PFDB.StatisticStructure;


/// <summary>
/// Interface for any collections defined for Phantom Forces.
/// </summary>
public interface IPFDBCollection
{
	/// <summary>
	/// Indicates whether any element in the file requires revision.
	/// </summary>
	public bool CollectionNeedsRevision { get; }

	/// <summary>
	/// Defines the unique weapon identifier for the collection.
	/// </summary>
	public WeaponIdentification WeaponID { get; }
}

