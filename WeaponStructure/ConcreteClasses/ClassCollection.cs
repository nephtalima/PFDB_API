using PFDB.WeaponUtility;
using System.Collections.Generic;
using PFDB.Logging;

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
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){});
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="classes">The collection of weapon classes to add.</param>
	public ClassCollection(IEnumerable<IClass> classes)
	{
		
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(classes), classes}
		});
		this.AddRange(classes);
	}

	/// <inheritdoc/>
	public new void Add(IClass classItem)
	{
		
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(classItem), classItem}
		});
		//todo: add checks
		base.Add(classItem);
	}

	/// <inheritdoc/>
	public new void AddRange(IEnumerable<IClass> classes)
	{
		
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(classes), classes}
		});
		//todo: add checks
		base.AddRange(classes);
	}

	/// <inheritdoc/>
	public void Add(IClassCollection classes)
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(classes), classes}
		});
		//todo: add checks
		base.AddRange(classes.Classes);
	}
}
