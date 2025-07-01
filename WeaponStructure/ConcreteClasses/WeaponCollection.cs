using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.WeaponStructure;


/// <summary>
/// Defines a collection of weapons, assumed to be in the same category. Serves as a wrapper for <see cref="List{IWeapon}"/> with additional fields. 
/// </summary>
public class WeaponCollection : List<IWeapon>, IWeaponCollection
{

	/// <inheritdoc/>
	public IEnumerable<IWeapon> Weapons => this;

	/// <summary>
	/// Retrieves the category of the collection. Assumes all the weapons are the same category.
	/// </summary>
	public Categories Category => this.First().Category;

	/// <inheritdoc/>
	public bool CollectionNeedsRevision
	{
		get
		{
			return this.Any(x => x.NeedsRevision);
		}
	}

	/// <summary>
	/// Default constructor.
	/// </summary>
	public WeaponCollection()
	{

	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="weapons">The list of weapons to add to the collection.</param>
	public WeaponCollection(IEnumerable<IWeapon> weapons)
	{
		this.AddRange(weapons);
	}

	/// <inheritdoc/>
	public new void Add(IWeapon weapon)
	{

		//todo: add checks
		base.Add(weapon);
	}

	/// <inheritdoc/>
	public new void AddRange(IEnumerable<IWeapon> weapons)
	{
		//todo: add checks
		base.AddRange(weapons);
	}

	/// <inheritdoc/>
	public void Add(IWeaponCollection weapons)
	{
		//todo: add checks
		base.AddRange(weapons.Weapons);
	}

}

