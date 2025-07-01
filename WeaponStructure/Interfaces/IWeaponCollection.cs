
namespace PFDB.WeaponUtility;


/// <summary>
/// Defines a collection of weapons.
/// </summary>
public interface IWeaponCollection : IPFDBCollection
{
	/// <summary>
	/// The underlying list of weapons.
	/// </summary>
	IEnumerable<IWeapon> Weapons { get; }

	/// <summary>
	/// Adds a weapon to the underlying list.
	/// </summary>
	/// <param name="weapon">The weapon to add.</param>
	void Add(IWeapon weapon);

	/// <summary>
	/// Adds a collection of weapons to the list.
	/// </summary>
	/// <param name="weapons">The collection of weapons to add.</param>
	void Add(IWeaponCollection weapons);

	/// <summary>
	/// Adds an enumerable list of categories to the lsit.
	/// </summary>
	/// <param name="weapons">The categories list of weapons to add.</param>
	void AddRange(IEnumerable<IWeapon> weapons);
}
