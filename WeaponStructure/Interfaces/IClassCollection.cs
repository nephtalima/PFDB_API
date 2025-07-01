
namespace PFDB.WeaponUtility;


/// <summary>
/// Defines an interface 
/// </summary>
public interface IClassCollection : IPFDBCollection
{
	/// <summary>
	/// The underlying collection of weapon classes.
	/// </summary>
	IEnumerable<IClass> Classes { get; }
	/// <summary>
	/// Adds a weapon class to the underlying list.
	/// </summary>
	/// <param name="classItem">The weapon class to add.</param>
	void Add(IClass classItem);
	/// <summary>
	/// Adds a collection of weapon classes to the list.
	/// </summary>
	/// <param name="classes">The collection of weapon classes to add.</param>
	void Add(IClassCollection classes);
	/// <summary>
	/// Adds an enumerable list of weapon classes to the lsit.
	/// </summary>
	/// <param name="classes">The weapon class list to add.</param>
	void AddRange(IEnumerable<IClass> classes);
}
