
namespace PFDB.WeaponUtility;


/// <summary>
/// Defines an interface for a collection of categories.
/// </summary>
public interface ICategoryCollection : IPFDBCollection
{
	/// <summary>
	/// The underlying collection of categories.
	/// </summary>
	IEnumerable<ICategory> Categories { get; }


	/// <summary>
	/// Adds a category to the underlying list.
	/// </summary>
	/// <param name="category">The category to add.</param>
	void Add(ICategory category);

	/// <summary>
	/// Adds a collection of categories to the list.
	/// </summary>
	/// <param name="categories">The collection of categories to add.</param>
	void Add(ICategoryCollection categories);


	/// <summary>
	/// Adds an enumerable list of categories to the lsit.
	/// </summary>
	/// <param name="categories">The categories list of weapons to add.</param>
	void AddRange(IEnumerable<ICategory> categories);
}
