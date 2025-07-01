using PFDB.WeaponUtility;

namespace PFDB.WeaponStructure;


/// <summary>
/// Defines a class for a weapon class.
/// </summary>
public class Class : IClass
{
	private Classes _classType;
	private ICategoryCollection _categories;

	/// <inheritdoc/>
	public bool NeedsRevision { get { return _categories.CollectionNeedsRevision; } }
	/// <inheritdoc/>
	public Classes ClassType { get { return _classType; } }
	/// <inheritdoc/>
	public ICategoryCollection CategoryCollection { get { return _categories; } }
	/// <summary>
	/// Default constructor.
	/// </summary>
	/// <param name="classType">The weapon class type.</param>
	/// <param name="categories">The underlying collection of categories that comprise the weapon class.</param>
	public Class(Classes classType, ICategoryCollection categories)
	{
		_classType = classType;
		_categories = categories;
	}
}
