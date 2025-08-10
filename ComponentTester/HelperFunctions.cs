using PFDB.WeaponUtility;
using PFDB.SQLite;

namespace PFDB;

public static class Helper
{
    public static Dictionary<Categories, List<int>> GetWeaponNumbersForSpecificVersion(PhantomForcesVersion version)
    {
        Dictionary<Categories, List<int>> weaponNumbers = new Dictionary<Categories, List<int>>();
        foreach (Categories category in WeaponTable.WeaponCounts[version].Keys)
        {
            List<int> temporaryList = new List<int>();

            for (int i = 0; i < (int)WeaponTable.WeaponCounts[version][category]; ++i)
            {
                temporaryList.Add(i);
            }

            weaponNumbers.Add(category, temporaryList);

        }
        return weaponNumbers;

    }


}