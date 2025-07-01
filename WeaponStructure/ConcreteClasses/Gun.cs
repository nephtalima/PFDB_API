using PFDB.Conversion;
using PFDB.ConversionUtility;
using PFDB.WeaponUtility;
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
	}

	/// <inheritdoc/>
	public Gun(string name, string? description, IConversionCollection conversionCollection, Categories category) : base(name, description, conversionCollection, category)
	{
	}
}
