using PFDB.WeaponUtility;

namespace PFDB.WeaponStructure;


/// <summary>
/// Defines a class for a collection of categories.
/// </summary>
public class CategoryCollection : List<ICategory>, ICategoryCollection
{

	/// <inheritdoc/>
	public IEnumerable<ICategory> Categories => this;

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
	public CategoryCollection()
	{

	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="categories">The list of categories to add to the collection.</param>
	public CategoryCollection(IEnumerable<ICategory> categories)
	{
		this.AddRange(categories);
	}

	/// <inheritdoc/>
	public new void Add(ICategory category)
	{
		//todo: add checks
		base.Add(category);
	}

	/// <inheritdoc/>
	public new void AddRange(IEnumerable<ICategory> categories)
	{
		//todo: add checks
		base.AddRange(categories);
	}

	/// <inheritdoc/>
	public void Add(ICategoryCollection categoryCollection)
	{
		//todo: add checks
		base.AddRange(categoryCollection.Categories);
	}
}
