using PFDB.ConversionUtility;
namespace PFDB.WeaponUtility;


/// <summary>
/// Defines an interface for easily defining weapons in the game.
/// </summary>
public interface IWeapon
{
	/// <summary>
	/// If the current weapon requries manual proofreading.
	/// </summary>
	bool NeedsRevision { get; }

	/// <summary>
	/// Contains a collection of objects that inherit from <see cref="IConversionCollection"/>.
	/// </summary>
	IConversionCollection ConversionCollection { get; }

	/// <summary>
	/// Defines the category of the weapon.
	/// </summary>
	Categories Category { get; }
}
