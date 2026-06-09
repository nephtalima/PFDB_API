using PFDB.Conversion;
using PFDB.ConversionUtility;
using PFDB.WeaponUtility;
using PFDB.Logging;
using System.Reflection.Metadata.Ecma335;

namespace PFDB.WeaponStructure;


/// <summary>
/// Defines a gun.
/// </summary>
public sealed class Gun : Weapon {

	/// <inheritdoc/>
	public Gun(string name, IConversionCollection conversionCollection, Categories category) : base(name, conversionCollection, category)
	{
		//Console.WriteLine(_conversionCollection.Conversions.First().StatisticCollection.Statistics.First().WeaponID.Version.VersionNumber);
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(name), name},
			{nameof(category), category}
		});
	}

	/// <inheritdoc/>
	public Gun(string name, string? description, IConversionCollection conversionCollection, Categories category) : base(name, description, conversionCollection, category)
	{
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(name), name},
			{nameof(category), category},
			{nameof(description), description}
		});
	}
}
