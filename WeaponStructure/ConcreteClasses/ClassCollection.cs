using PFDB.WeaponUtility;
using System.Collections.Generic;

namespace PFDB.WeaponStructure;


/// <summary>
/// Defines a class that contains a collection of weapon classes.
/// </summary>
public class ClassCollection : List<IClass>, IClassCollection
{
	/// <inheritdoc/>
	public bool CollectionNeedsRevision
	{
		get
		{
			return this.Any(x => x.NeedsRevision);
		}
	}
	/// <inheritdoc/>
	public IEnumerable<IClass> Classes => this;

	/// <summary>
	/// Default constuctor.
	/// </summary>
	public ClassCollection()
	{

	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="classes">The collection of weapon classes to add.</param>
	public ClassCollection(IEnumerable<IClass> classes)
	{
		this.AddRange(classes);
	}

	/// <inheritdoc/>
	public new void Add(IClass classItem)
	{
		//todo: add checks
		base.Add(classItem);
	}

	/// <inheritdoc/>
	public new void AddRange(IEnumerable<IClass> classes)
	{
		//todo: add checks
		base.AddRange(classes);
	}

	/// <inheritdoc/>
	public void Add(IClassCollection classes)
	{
		//todo: add checks
		base.AddRange(classes.Classes);
	}
}
