using PFDB.ConversionUtility;
using PFDB.WeaponUtility;

namespace PFDB.WeaponStructure;


/// <summary>
/// Defines the base class for all weapons in the game.
/// </summary>
public abstract class Weapon : IWeapon
{

	private protected string _name;
	private protected string? _description;
	private protected IConversionCollection _collection;

	/// <inheritdoc/>
	public Categories Category { get; }

	/// <inheritdoc/>
	public virtual IConversionCollection ConversionCollection { get { return _collection; } }


	/// <inheritdoc/>
	public bool NeedsRevision => ConversionCollection.CollectionNeedsRevision;

	/// <summary>
	/// Constructor for the base weapon class. By default, description is set to null.
	/// </summary>
	/// <param name="name">The name of the weapon.</param>
	/// <param name="collection">The collection of conversions for the weapon.</param>
	/// <param name="category">The category of the weapon.</param>
	public Weapon(string name, IConversionCollection collection, Categories category)
	{
		_collection = collection;
		_name = name; _description = null;
		Category = category;
	}

	/// <summary>
	/// Constructor for the base weapon class.
	/// </summary>
	/// <param name="name">The name of the weapon.</param>
	/// <param name="description">The description of the weapon. Optional.</param>
	/// <param name="collection">The collection of conversions for the weapon.</param>
	/// <param name="category">The category of the weapon.</param>
	public Weapon(string name, string? description, IConversionCollection collection, Categories category)
	{
		_collection = collection;
		_name = name;
		_description = description;
		Category = category;
	}
}