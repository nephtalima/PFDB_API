using PFDB.Conversion;
using PFDB.ConversionUtility;
using PFDB.StatisticUtility;
using PFDB.WeaponUtility;
using PFDB.Logging;
using System.Collections.Generic;

namespace PFDB.WeaponStructure;


/// <summary>
/// Defines a melee.
/// </summary>
public sealed class Melee : Weapon
{

	/// <inheritdoc/>
	public Melee(string name, IDefaultConversion defaultConversion, Categories category) : base(name, new ConversionCollection(defaultConversion), category)
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(name), name},
			{nameof(defaultConversion), defaultConversion},
			{nameof(category), category}
		});
	}

	/// <inheritdoc/>
	public Melee(string name, string? description, IDefaultConversion defaultConversion, Categories category) : base(name, description, new ConversionCollection(defaultConversion), category)
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(name), name},
			{nameof(defaultConversion), defaultConversion},
			{nameof(category), category},
			{nameof(description), description}
		});
	}
}