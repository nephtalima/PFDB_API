using PFDB.PythonExecutionUtility;
using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.PythonExecution;


/// <summary>
/// Defines a class to verify the rank of a weapon through reading its image. Inherits from <see cref="PythonTesseractExecutable"/>.
/// </summary>
public class PythonRankVerifierExecutable : PythonTesseractExecutable, IPythonExecutable
{

	internal PythonRankVerifierExecutable() : base() { }

	private static bool _isWindows = Directory.Exists("C:/");
	private static bool _isLinux = File.Exists("/boot/vmlinuz-linux");
	/// <inheritdoc/>
	public override ProcessStartInfo GetProcessStartInfo()
	{
		ProcessStartInfo pyexecute;
		StringBuilder command = new StringBuilder("Command used: ");
		if (TessbinPath == null)
		{
			pyexecute = new ProcessStartInfo(ProgramDirectory + "impa" + (_isWindows ? ".exe" : string.Empty), $"-cr {FileDirectory + Filename} {Convert.ToString((int)WeaponType)} {WeaponID.Version.VersionNumber.ToString()}");
			command.Append(pyexecute.Arguments);
			command = command.Replace(FileDirectory + Filename, "...." + PyUtilityClass.CommonExecutionPath(Directory.GetCurrentDirectory() ?? "null", FileDirectory + Filename).relativeForeignPath);
		}
		else
		{
			pyexecute = new ProcessStartInfo(ProgramDirectory + "impa" + (_isWindows ? ".exe" : string.Empty), $"-fr {TessbinPath} {FileDirectory + Filename} {Convert.ToString((int)WeaponType)} {WeaponID.Version.VersionNumber.ToString()}");
			command.Append(pyexecute.Arguments);
			command = command.Replace(TessbinPath, "...." + PyUtilityClass.CommonExecutionPath(Directory.GetCurrentDirectory() ?? "null", TessbinPath).relativeForeignPath);
		}
		_commandExecuted = command.ToString();
		pyexecute.RedirectStandardOutput = true;
		pyexecute.UseShellExecute = false;
		return pyexecute;
	}

}
