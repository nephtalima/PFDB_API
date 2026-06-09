using PFDB.WeaponUtility;
using PFDB.Logging;
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
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){});
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="weapons">The list of weapons to add to the collection.</param>
	public WeaponCollection(IEnumerable<IWeapon> weapons)
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(weapons), weapons}
		});
		this.AddRange(weapons);
	}

	/// <inheritdoc/>
	public new void Add(IWeapon weapon)
	{
		
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(weapon), weapon}
		});
		//todo: add checks
		base.Add(weapon);
	}

	/// <inheritdoc/>
	public new void AddRange(IEnumerable<IWeapon> weapons)
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(weapons), weapons}
		});
		//todo: add checks
		base.AddRange(weapons);
	}

	/// <inheritdoc/>
	public void Add(IWeaponCollection weapons)
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(weapons), weapons}
		});
		//todo: add checks
		base.AddRange(weapons.Weapons);
	}

}

