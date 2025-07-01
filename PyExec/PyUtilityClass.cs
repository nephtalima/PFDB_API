using System;
using System.IO;

namespace PFDB.PythonExecution;


/// <summary>
/// General utility class for Python interop.
/// </summary>
public static class PyUtilityClass
{
	/// <summary>
	/// Changes slash based on if the operating system is Linux or Windows.
	/// </summary>
	public static char slash = Directory.Exists("/usr/bin") ? '/' : '\\';



	/// <summary>
	/// Compares two file paths and determines a common directory (excludes absolute if a common one is found). Can be used to obfuscate parent root directories when they are not needed. Currently slightly broken, but this function is completely optional tbh
	/// </summary>
	/// <param name="currentProcessPath">Current directory. Note that this parameter need not be the actual current process directory.</param>
	/// <param name="foreignPath">Foreign directory. Note that this parameter need not be the actual foreign directory.</param>
	/// <returns>A <c>Tuple</c>, with the first item containing the path from the common path to the current directory path, and the second item containing the path from the common path to the foreign directory.</returns>
	/// <exception cref="Exception">Throws exceptions if two paths are unequal when they should be. (illegal case)</exception>
	public static (string relativeCurrentPath, string relativeForeignPath) CommonExecutionPath(string currentProcessPath, string foreignPath)
	{
		string tempCurrent = currentProcessPath;
		string tempForeign = foreignPath;
		if (!currentProcessPath.StartsWith(foreignPath) && !foreignPath.StartsWith(currentProcessPath))
		{ //distinct, but has common directory
			for (int i = 0; i < Math.Min(currentProcessPath.Length, foreignPath.Length); ++i)
			{
				if (currentProcessPath[i] != foreignPath[i])
				{
					try
					{
						tempCurrent = tempCurrent.Substring(0, i);
						tempForeign = tempForeign.Substring(0, i);

						if (tempForeign != tempCurrent) throw new Exception($"Something went really wrong: {tempCurrent} should equal {tempForeign}");

						int lastSlashBeforeSubstringC = tempCurrent.LastIndexOf(slash); //finds last slash (aka last common directory)
						int lastSlashBeforeSubstringF = tempForeign.LastIndexOf(slash);

						if (lastSlashBeforeSubstringC != lastSlashBeforeSubstringF) throw new Exception($"Something went really wrong: {lastSlashBeforeSubstringC} should equal {lastSlashBeforeSubstringF}");

						tempCurrent = tempCurrent.Substring(0, lastSlashBeforeSubstringC); //truncates off last slash
						tempForeign = tempForeign.Substring(0, lastSlashBeforeSubstringF);

						if (tempForeign != tempCurrent) throw new Exception($"Something went really wrong: {tempCurrent} should equal {tempForeign}");


						int lastSlashBeforeSubstringC2 = tempCurrent.LastIndexOf(slash);
						int lastSlashBeforeSubstringF2 = tempForeign.LastIndexOf(slash);

						tempCurrent = currentProcessPath.Substring(lastSlashBeforeSubstringC2);
						tempForeign = foreignPath.Substring(lastSlashBeforeSubstringF2);
					}
					catch
					{
						break; //it's not important
					}
					break;
				}
			}
		}
		else //subset, or the same
		{
			try
			{
				tempCurrent = tempCurrent.Substring(0, Math.Min(currentProcessPath.Length, foreignPath.Length));
				tempForeign = tempForeign.Substring(0, Math.Min(currentProcessPath.Length, foreignPath.Length));

				int lastSlashBeforeSubstringC = tempCurrent.LastIndexOf(slash); //finds last slash (aka last common directory)
				int lastSlashBeforeSubstringF = tempForeign.LastIndexOf(slash);
				if (lastSlashBeforeSubstringC == -1) lastSlashBeforeSubstringC = 0; //couldn't find slash in currentDir, make it beginning of string
				if (lastSlashBeforeSubstringF == -1) lastSlashBeforeSubstringF = 0;

				tempCurrent = tempCurrent.Substring(0, lastSlashBeforeSubstringC); //truncates off last slash
				tempForeign = tempForeign.Substring(0, lastSlashBeforeSubstringF);
				//alternative: tempForeign = tempForeign[..lastSlashBeforeSubstringF];

				int lastSlashBeforeSubstringC2 = tempCurrent.LastIndexOf(slash);
				int lastSlashBeforeSubstringF2 = tempForeign.LastIndexOf(slash);
				if (lastSlashBeforeSubstringC2 == -1) lastSlashBeforeSubstringC2 = 0; //couldn't find slash in currentDir, make it beginning of string
				if (lastSlashBeforeSubstringF2 == -1) lastSlashBeforeSubstringF2 = 0;

				tempCurrent = currentProcessPath.Substring(lastSlashBeforeSubstringC2);
				tempForeign = foreignPath.Substring(lastSlashBeforeSubstringF2);
			}
			catch
			{
				//do nothing, not important
			}
		}
		return (tempCurrent, tempForeign);
	}
}