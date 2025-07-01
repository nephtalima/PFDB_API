using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFDB.WeaponUtility;

namespace PFDB.WeaponStructure;


/// <summary>
/// Defines a class for categories
/// </summary>
public class Category : ICategory
{
	private Categories _categoryType;
	private IWeaponCollection _weaponCollection;

	/// <summary>
	/// Name of the category.
	/// </summary>
	public string CategoryName { get { return nameof(_categoryType); } }

	/// <inheritdoc/>
	public Categories CategoryType { get { return _categoryType; } }
	/// <inheritdoc/>
	public bool NeedsRevision { get { return _weaponCollection.CollectionNeedsRevision; } }
	/// <inheritdoc/>
	public IWeaponCollection WeaponCollection { get { return _weaponCollection; } }

	/// <summary>
	/// Constructor for a new category object.
	/// </summary>
	/// <param name="category">The type of category.</param>
	/// <param name="weaponCollection">The underlying weapon collection for the category.</param>
	public Category(Categories category, IWeaponCollection weaponCollection)
	{
		_weaponCollection = weaponCollection;
		_categoryType = category;

	}
}
