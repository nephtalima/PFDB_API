using PFDB.WeaponUtility;
using PFDB.ConversionUtility;
using PFDB.PythonExecutionUtility;
using PFDB.StatisticStructure;

namespace PFDB.Conversion;

/// <summary>
/// Defines a single default conversion for weapons.
/// </summary>
public class DefaultConversion : Conversion, IDefaultConversion
{
	/// <inheritdoc/>
	public DefaultConversion(IStatisticCollection statisticCollection) : base(statisticCollection)
	{
	}

	/// <inheritdoc/>
	public DefaultConversion(string filename, WeaponIdentification weaponID) : base(filename, weaponID)
	{
		
	}

	/// <inheritdoc/>
	public DefaultConversion(IPythonExecutor pythonExecutor) : base(pythonExecutor) { }
}
