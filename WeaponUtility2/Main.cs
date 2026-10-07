using System;
using PFDB.Logging;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using PFDB.WeaponUtility;

namespace PFDB.WeaponUtilityTesting;

/// <summary>
/// Defines a class that tests functions within WeaponUtility
/// </summary>
public static class WeaponTest
{

    /// <summary>
    /// Main entry point.
    /// </summary>
    public static void Main(){

		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
        PFDBLogger logger = new PFDBLogger(".pfdblog");

    }

    /// <summary>
    /// Main testing function.
    /// </summary>
    /// <returns>True if all the tests pass, false otherwise.</returns>
    public static bool Test(){

		PFDBLogger.LogArguments(new Dictionary<string, object?>() {});
        int score = 0;
        PFDBLogger.LogInformation("");
        PFDBLogger.LogInformation($"\u001b[1;36mStarting Weapon Utility Testing.\u001b[0;0m");
        PFDBLogger.LogInformation("");
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if(PhantomForcesVersionConstructorTest())score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if(PhantomForcesVersionLegacyTest())score++;
        /*
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        */
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        bool pass = TestingOutput("All WeaponUtiltiy tests", score >= 2, "2", score.ToString());
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        PFDBLogger.LogInformation("");
        
        return pass;
    }


    /// <summary>
    /// Tests passing in individual numbers for version number; checks if the constructor makes the expected version string.
    /// </summary>
    /// <returns>True if all the tests pass, false otherwise.</returns>
    public static bool PhantomForcesVersionConstructorTest(){

        int majorVersion = 10;
        int minorVersion = 0;
        int revisionVersion = 1;

        PhantomForcesVersion version = new PhantomForcesVersion(majorVersion, minorVersion, revisionVersion);
        return TestingOutput("PhantomForcesVersion major, minor, revision number -> string constructor test", version.VersionString == $"{majorVersion}.{minorVersion}.{revisionVersion}", $"{majorVersion}.{minorVersion}.{revisionVersion}", version.VersionString);
    }


    /// <summary>
    /// Tests if certain versions (any version before version 9.0.0) are considered "Legacy" versions.
    /// </summary>
    /// <returns>True if all the tests pass, false otherwise.</returns>
    public static bool PhantomForcesVersionLegacyTest(){
        int score = 0;
        PhantomForcesVersion version1 = new PhantomForcesVersion("10.0.1");
        PhantomForcesVersion version2 = new PhantomForcesVersion("8.0.1");
        
        if(TestingOutput("PFV legacy negative test", version1.IsLegacy == false, $"{false}", $"{version1.IsLegacy}"))score++;
        if(TestingOutput("PFV legacy positive test", version2.IsLegacy == true, $"{true}", $"{version2.IsLegacy}"))score++;

        return TestingOutput("PhantomForcesVersion legacy tests", score >= 2, $"{2}", $"{score}");
    }



    /// <summary>
    /// Standardized way of outputting pass/fail condition for various tests.
    /// </summary>
    /// <param name="testName">Name of the test being performed.</param>
    /// <param name="pass">Whether the test passed or failed.</param>
    /// <param name="expectedOutput">Expected output (in string format).</param>
    /// <param name="actualOutput">Actual output (in string format).</param>
    /// <param name="caller">Leave blank unless you wish to override the original test function name.</param>
    /// <returns>Whether the test passed or failed (equivalent to the value of "pass".)</returns>
    public static bool TestingOutput(string testName, bool pass, string expectedOutput, string actualOutput, [CallerMemberName] string caller = "")
    {

		PFDBLogger.LogArguments(new Dictionary<string, object?>() {
			{nameof(testName), testName},
            {nameof(pass), pass},
            {nameof(expectedOutput), expectedOutput},
            {nameof(actualOutput), actualOutput},
            {nameof(caller), caller}
		});
        string originalCaller = caller ?? "";
        if (pass)
        {
            PFDBLogger.LogInformation($"{testName}\u001b[1;32m passed.\u001b[0;0m Expected: {expectedOutput}. Got: {actualOutput}", originalCaller);
            return true;
        }
        else
        {
            PFDBLogger.LogError($"{testName}\u001b[1;31m failed.\u001b[0;0m Expected: {expectedOutput}. Got: {actualOutput}", originalCaller);
            return false;
        }
    }
}