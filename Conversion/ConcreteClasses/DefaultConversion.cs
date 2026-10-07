using PFDB.WeaponUtility;
using PFDB.ConversionUtility;
using PFDB.PythonExecutionUtility;
using PFDB.StatisticStructure;
using System.Collections.Generic;
using PFDB.Logging;

namespace PFDB.Conversion;

/// <summary>
/// Defines a single default conversion for weapons.
/// </summary>
public class DefaultConversion : Conversion, IDefaultConversion
{
	/// <inheritdoc/>
	public DefaultConversion(IStatisticCollection statisticCollection) : base(statisticCollection)
	{
		
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(statisticCollection), statisticCollection}
		});
	}

	/// <inheritdoc/>
	public DefaultConversion(string filename, WeaponIdentification weaponID) : base(filename, weaponID)
	{
		
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(filename), filename},
			{nameof(weaponID), weaponID.ID}
		});
	}

	/// <inheritdoc/>
	public DefaultConversion(IPythonExecutor pythonExecutor) : base(pythonExecutor) {
		
		PFDBLogger.LogArguments(new Dictionary<string, object?>(){
			{nameof(pythonExecutor), pythonExecutor}
		});
	}
}
