using PFDB.Conversion;
using PFDB.ConversionUtility;
using PFDB.StatisticUtility;
using PFDB.WeaponUtility;
using PFDB.Logging;

namespace PFDB.WeaponStructure;


/// <summary>
/// Defines a grenade.
/// </summary>
public sealed class Grenade : Weapon
{
	/// <inheritdoc/>
	public Grenade(string name, IDefaultConversion defaultConversion, Categories category) : base(name, new ConversionCollection(defaultConversion), category)
	{

		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(name), name},
			{nameof(defaultConversion), defaultConversion},
			{nameof(category), category}
		});
	}
	/// <inheritdoc/>
	public Grenade(string name, string? description, IDefaultConversion defaultConversion, Categories category) : base(name, description, new ConversionCollection(defaultConversion), category)
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(name), name},
			{nameof(defaultConversion), defaultConversion},
			{nameof(category), category},
			{nameof(description), description}
		});
	}
}