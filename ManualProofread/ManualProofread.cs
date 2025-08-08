using PFDB.StatisticStructure;
using PFDB.WeaponStructure;
using PFDB.WeaponUtility;
using PFDB.ConversionUtility;
using PFDB.Logging;
using PFDB.ManualProofread;

ManualProofread proofread = new ManualProofread(PhantomForcesDataModel.GetWeaponCollection(new PhantomForcesVersion("10.1.1")));


namespace PFDB{
    namespace ManualProofread{
		class ManualProofread{
			public ManualProofread(IWeaponCollection weaponcollection){
				foreach (IWeapon weapon in weaponcollection.Weapons)
				{
					if (weapon.NeedsRevision == false) continue;
					foreach(IConversion conversion in weapon.ConversionCollection.Conversions)
					{
						if(conversion.NeedsRevision == false) continue;
						foreach(IStatistic statistic in conversion.StatisticCollection.Statistics)
						{
							if(statistic.NeedsRevision == false) continue;
							foreach(string str in statistic.Statistics)
							{
								Console.WriteLine(str);
								string newstr = Console.ReadLine() ?? str;
								Console.WriteLine($"The new statistic has {(str == newstr ? "not" : "")} been altered{(str == newstr ? "." : "to " + newstr)}");
								PFDBLogger.LogDebug($"The new statistic has {(str == newstr ? "not" : "")} been altered{(str == newstr ? "." : "to " + newstr)}", parameter: [ str, newstr]);
							}
						}
					}
				}

			}
			


		}
	
    }
}