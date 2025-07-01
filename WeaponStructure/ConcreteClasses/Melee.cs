using PFDB.Conversion;
using PFDB.ConversionUtility;
using PFDB.StatisticUtility;
using PFDB.WeaponUtility;

namespace PFDB.WeaponStructure;


/// <summary>
/// Defines a melee.
/// </summary>
public sealed class Melee : Weapon
{

	/// <inheritdoc/>
	public Melee(string name, IDefaultConversion defaultConversion, Categories category) : base(name, new ConversionCollection(defaultConversion), category)
	{
	}

	/// <inheritdoc/>
	public Melee(string name, string? description, IDefaultConversion defaultConversion, Categories category) : base(name, description, new ConversionCollection(defaultConversion), category)
	{
	}
}