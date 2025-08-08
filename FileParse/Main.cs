using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFDB;
using PFDB.Logging;
using PFDB.Parsing;
using PFDB.StatisticStructure;
using PFDB.AutomaticProofreading;
using PFDB.StatisticUtility;
using PFDB.WeaponUtility;
using System.Runtime.CompilerServices;
using System.IO;
using static PFDB.Parsing.DefaultStatisticParameters;

namespace PFDB.ParsingUtility;


/// <summary>
/// Defines a class that tests functions within FileParse.
/// </summary>
public static class ParseTesting {
    /// <summary>
    /// Main entry point.
    /// </summary>
    public static void Main()
    {
        PFDBLogger logger = new PFDBLogger(".pfdblog");
        //testing indexsearch
        Test(AcceptableSpaces, AcceptableCorruptedWordSpaces, StringComparisonMethod);
        return;
    }

    /// <summary>
    /// Main testing function.
    /// </summary>
    /// <param name="acceptableSpaces"></param>
    /// <param name="acceptableCorruptedWordSpaces">Specifies the number of </param>
    /// <param name="stringComparisonMethod"></param>
    public static bool Test(int acceptableSpaces, int acceptableCorruptedWordSpaces, StringComparison stringComparisonMethod /*= StringComparison.InvariantCultureIgnoreCase*/)
    {
        int score = 0;
        PFDBLogger.LogInformation("\u001b[1;35mBeginning parse testing:");
        PFDBLogger.LogInformation("");
        if (IndexSearchSingleCharacterTest(stringComparisonMethod)) score++;
        if (IndexSearchSingleWordTest(stringComparisonMethod)) score++;
        if (IndexSearchNoOccurencesTest(stringComparisonMethod)) score++;
        if (CorruptedWordFixTest(acceptableSpaces, acceptableCorruptedWordSpaces, stringComparisonMethod)) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if (StatisticParseMatchTest(acceptableSpaces, acceptableCorruptedWordSpaces, stringComparisonMethod)) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if (SearchTargetToStatisticOptionTest()) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if (StatisticOptionToSearchTargetTest()) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if (FileReaderTest()) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if (FileParseCompleteTest(acceptableSpaces, acceptableCorruptedWordSpaces, stringComparisonMethod)) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        if (AddingIncompatibleWeaponsToStatisticCollectionTest()) score++;
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        bool pass = TestingOutput("All FileParse tests", score >= 10, "10", score.ToString());
        PFDBLogger.LogInformation("________________");
        PFDBLogger.LogInformation("");
        PFDBLogger.LogInformation("");
        return pass;

    }

    /// <summary>
    /// Tests if <see cref="IndexSearch"/> can accurately find locations of a single word.
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool IndexSearchSingleWordTest(StringComparison stringComparisonMethod) {
        IIndexSearch testing1 = new IndexSearch(
            "the quick brown fox jumps over the lazy fox",
            "fox", stringComparisonMethod);
        int score1 = 0;
        List<int> expected1 = new List<int>() { 16, 40 };
        foreach (int y in expected1) {
            if (testing1.ListOfIndices.Contains(y)) score1++;
        }
        StringBuilder builder = new StringBuilder();
        foreach (int x in testing1.ListOfIndices) builder.Append(x + " ");

        return TestingOutput("Index search single word test", score1 >= 2, "40 16", builder.ToString());
    }

    /// <summary>
    /// Tests if <see cref="IndexSearch"/> can accurately find locations of a single character. 
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool IndexSearchSingleCharacterTest(StringComparison stringComparisonMethod) {
        IIndexSearch testing1 = new IndexSearch(
            "the quick brown fox jumps over the lazy dog",
            "o", stringComparisonMethod);
        int score1 = 0;
        List<int> expected1 = new List<int>() { 12, 17, 26, 41 };
        foreach (int y in expected1) {
            if (testing1.ListOfIndices.Contains(y)) score1++;
        }
        StringBuilder builder = new StringBuilder();
        foreach (int x in testing1.ListOfIndices) builder.Append(x + " ");

        return TestingOutput("Index search single character test", score1 >= 3, "41 26 17 12", builder.ToString());
    }

    /// <summary>
    /// Tests if <see cref="IndexSearch"/> can detect when there are no occurences of a search.
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool IndexSearchNoOccurencesTest(StringComparison stringComparisonMethod) {
        IIndexSearch testing1 = new IndexSearch(
            "the quick brown fox jumps over the lazy dog",
            "1", stringComparisonMethod);
        StringBuilder builder = new StringBuilder();
        foreach (int x in testing1.ListOfIndices) builder.Append(x + " ");
        return TestingOutput("Index search no occurence test", testing1.ListOfIndices.Any() == false, "<no items>",
            (builder.ToString() == string.Empty || builder.ToString() == " ") ? "<no items>" : builder.ToString());
    }

    /// <summary>
    /// Tests if <see cref="StatisticParse._corruptedWordFixer(string?, StringComparison)"/> can fix a word spelt wrong (common from PyTesseract scans). 
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool CorruptedWordFixTest(int acceptableSpaces, int acceptableCorruptedWordSpaces, StringComparison stringComparisonMethod)
    {
        StatisticParse AN94 = new StatisticParse(new WeaponIdentification(new PhantomForcesVersion("10.0.1"), Categories.AssaultRifles, 11, 0, "AN-94"), sampleText, acceptableSpaces, acceptableCorruptedWordSpaces);
        string corruptedWord = AN94._corruptedWordFixer("CAPACITY", stringComparisonMethod);
        sampleText = sampleText.Replace(corruptedWord.TrimEnd(), "CAPACITY");
        return TestingOutput("Corrupted word fixing test. Testing if \"CAPAClTY\" gets changed to \"CAPACITY\"", AN94.Filetext.Contains("CAPACITY"), "True", AN94.Filetext.Contains("CAPACITY").ToString());
    }

    /// <summary>
    /// Tests if <see cref="StatisticParse.FindStatisticInFile(SearchTargets, IEnumerable{char}, StringComparison)"/> can successfully detect and add a statistic from a file.
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool StatisticParseMatchTest(int acceptableSpaces, int acceptableCorruptedWordSpaces, StringComparison stringComparisonMethod) {
        StatisticParse AN94 = new StatisticParse(new WeaponIdentification(new PhantomForcesVersion("10.0.1"), Categories.AssaultRifles, 11, 0, "AN-94"), sampleText, acceptableSpaces, acceptableCorruptedWordSpaces);
        SearchTargets target = SearchTargets.WeaponWalkspeed;
        bool status = false;
        PFDBLogger.LogInformation($"StatisticParse match test. Current target: {target}");
        try
        {
            string stat = AN94.FindStatisticInFile(target, ['\n', '\r'], stringComparisonMethod);
            PFDBLogger.LogInformation($"Successfully added test statistic ({stat}) to StatisticParse object.");
            status = true;
        }
        catch (ArgumentException)
        {
            PFDBLogger.LogInformation($"Test statistic ({target}) was not added to StatisticParse object.");
        }
        catch (WordNotFoundException)
        {
            PFDBLogger.LogError($"Whoever wrote the sample text is a moron. :(\n{sampleText}");
        }
        return TestingOutput("StatisticParse match test.", status, true.ToString(), status.ToString());

    }

    /// <summary>
    /// Tests if <see cref="StatisticCollection"/> will detect if an incompatible statistic addition is attempted.
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool AddingIncompatibleWeaponsToStatisticCollectionTest() {
        int score = 0;

        WeaponIdentification G11K2 = new WeaponIdentification(
            new PhantomForcesVersion("10.0.1"),
            Categories.AssaultRifles,
            211,
            0,
            "G11K2"
        );

        IStatistic statisticTest = new Statistic(false, "15.00", G11K2, StatisticOptions.PenetrationDepth);

        IStatisticCollection statisticCollection = new StatisticCollection(G11K2);
        statisticCollection.Add(statisticTest);
        try {
            statisticCollection.Add(new Statistic(false, "10.00", new WeaponIdentification(new PhantomForcesVersion("10.1.0"), Categories.PersonalDefenseWeapons, 0, 0, "MP5K"), StatisticOptions.ReloadTime));
            PFDBLogger.LogError("\u001b[0;31mUnsuccessfully\u001b[0;0m caught the previous exception when executing StatisticCollection.Add(IEnumerable<IStatistic>)");
        } catch (ArgumentException) {
            PFDBLogger.LogInformation("\u001b[0;32mSuccessfully\u001b[0;0m caught the previous exception when executing StatisticCollection.Add(IStatistic)");
            score++;
        }
        try
        {
            statisticCollection.AddRange(new List<IStatistic>() { new Statistic(false, "10.00", new WeaponIdentification(new PhantomForcesVersion("10.1.0"), Categories.PersonalDefenseWeapons, 0, 0, "MP5K"), StatisticOptions.ReloadTime) });
            PFDBLogger.LogError("\u001b[0;31mUnsuccessfully\u001b[0;0m caught the previous exception when executing StatisticCollection.AddRange(IEnumerable<IStatistic>)");
        }
        catch (ArgumentException)
        {
            PFDBLogger.LogInformation("\u001b[0;32mSuccessfully\u001b[0;0m caught the previous exception when executing StatisticCollection.AddRange(IEnumerable<IStatistic>)");
            score++;
        }

        return score >= 2;
    }

    /// <summary>
    /// Tests if <see cref="StatisticProofread.StatisticOptionToSearchTarget(StatisticOptions)"/> successfully converts <see cref="StatisticOptions"/> to <see cref="SearchTargets"/>.   
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool StatisticOptionToSearchTargetTest() {
        List<StatisticOptions> options = new List<StatisticOptions>(){
                    StatisticOptions.TotalAmmoCapacity,
                    StatisticOptions.MagazineCapacity,
                    StatisticOptions.ReserveCapacity,
                    StatisticOptions.HeadMultiplier,
                    StatisticOptions.BlastRadius,
                    StatisticOptions.FrontStabDamage,
                    StatisticOptions.EmptyReloadTime
                };
        List<SearchTargets> targets = new List<SearchTargets>(){
                    SearchTargets.AmmoCapacity,
                    SearchTargets.AmmoCapacity,
                    SearchTargets.AmmoCapacity,
                    SearchTargets.HeadMultiplier,
                    SearchTargets.BlastRadius,
                    SearchTargets.FrontStabDamage,
                    SearchTargets.EmptyReloadTime
                };
        int score = 0;
        for (int i = 0; i < options.Count; ++i) {
            SearchTargets value = StatisticProofread.StatisticOptionToSearchTarget(options[i]);
            bool pass = value == targets[i];
            TestingOutput($"Converting {options[i]} to {targets[i]}", pass, targets[i].ToString(), value.ToString());
            if (pass) {
                score++;
            }
        }
        return TestingOutput("All conversions from StatisticOptions to SearchTargets", score >= 7, "7", score.ToString());
    }

    /// <summary>
    /// Tests if <see cref="StatisticProofread.SearchTargetToStatisticOption(SearchTargets)"/> successfully converts <see cref="SearchTargets"/> to <see cref="StatisticOptions"/>.   
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool SearchTargetToStatisticOptionTest() {
        List<StatisticOptions> options = new List<StatisticOptions>(){
                    StatisticOptions.TotalAmmoCapacity,
                    StatisticOptions.MagazineCapacity,
                    StatisticOptions.ReserveCapacity,
                    StatisticOptions.HeadMultiplier,
                    StatisticOptions.BlastRadius,
                    StatisticOptions.FrontStabDamage,
                    StatisticOptions.EmptyReloadTime
                };
        List<SearchTargets> targets = new List<SearchTargets>(){
                    SearchTargets.AmmoCapacity,
                    SearchTargets.AmmoCapacity,
                    SearchTargets.AmmoCapacity,
                    SearchTargets.HeadMultiplier,
                    SearchTargets.BlastRadius,
                    SearchTargets.FrontStabDamage,
                    SearchTargets.EmptyReloadTime
                };
        int score = 0;
        for (int i = 0; i < targets.Count; ++i) {
            StatisticOptions value;
            try {
                value = StatisticProofread.SearchTargetToStatisticOption(targets[i]);
            } catch (ArgumentException) {
                bool caught = targets[i] == SearchTargets.AmmoCapacity;
                TestingOutput("Testing if ArgumentException is thrown when passing SearchTargets.AmmoCapacity", caught, SearchTargets.AmmoCapacity.ToString(), targets[i].ToString());
                if (caught) score++;
                continue;
            }
            bool pass = value == options[i];
            TestingOutput($"Converting {targets[i]} to {options[i]}", pass, options[i].ToString(), value.ToString());
            if (pass) {
                score++;
            }
        }
        return TestingOutput("All conversions from SearchTargets to StatisticOptions", score >= 7, "7", score.ToString());
    }

    /// <summary>
    /// Tests if <see cref="FileParse.FileReader(string)"/> can read a newly created file.
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool FileReaderTest() {
        int score = 0;

        Guid guid = Guid.NewGuid();
        string fileName = $"{guid}.txt";
        string filePath = $"./{guid}.txt";
        WeaponIdentification AN94 = new WeaponIdentification(new PhantomForcesVersion("10.0.1"), Categories.AssaultRifles, 11, 0, "AN-94");
        FileParse parser = new FileParse(AN94);

        try {
            parser.FileReader($"{guid}.error");
            PFDBLogger.LogError($"{guid}.error should not exist. Run this test again.");
        } catch (FileNotFoundException) {
            PFDBLogger.LogInformation("\u001b[0;32mSuccessfully caught previous exception (FileNotFoundException).");
            score++;
        } catch (Exception e) {
            PFDBLogger.LogWarning($"Something bad happened. Was expecting FileNotFoundException but got another exception instead. Message: {e.Message}");
        }

        try {
            File.WriteAllText(fileName, sampleText);
            PFDBLogger.LogInformation($"Created file {filePath}");
            score++;
            parser.FileReader(filePath);
            PFDBLogger.LogInformation($"Read file {filePath}");
            score++;
            File.Delete(filePath);
            PFDBLogger.LogInformation($"Deleted file {filePath}");
            PFDBLogger.LogInformation($"Successfully created and read a file ({filePath}) in the target filepath.");
        } catch {
            PFDBLogger.LogError($"{filePath} does not exist or cannot be written to. Check if read and write privileges are enabled correctly (i.e. the current user running this program should be able to modify the directory and create a new file.)");
        }
        return TestingOutput("FileParse reading test", score >= 3, $"{3}", $"{score}");
    }

    /// <summary>
    /// Tests if <see cref="FileParse"/> can take a sample output from PyExec and successfully parse it into collections of statistics. 
    /// </summary>
    /// <returns>Whether this test passes.</returns>
    public static bool FileParseCompleteTest(int acceptableSpaces, int acceptableCorruptedWordSpaces, StringComparison stringComparisonMethod) {
        WeaponIdentification AN94 = new WeaponIdentification(new PhantomForcesVersion("10.0.1"), Categories.AssaultRifles, 11, 0, "AN-94");
        FileParse AN94parse = new FileParse(AN94, sampleText);

        StatisticCollection collection = (StatisticCollection)AN94parse.FindAllStatisticsInFileWithTypes(acceptableSpaces, acceptableCorruptedWordSpaces, stringComparisonMethod);
        int score = 0;
        foreach (Statistic stat in collection) {
            switch (stat.Option) {
                case StatisticOptions.Rank:
                    if (stat.Statistics.First().TrimEnd() == "11") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.Firerate:
                    if (stat.Statistics.First().TrimEnd() == "600A") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.TotalAmmoCapacity:
                    if (stat.Statistics.First().TrimEnd() == "150") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.MagazineCapacity:
                    if (stat.Statistics.First().TrimEnd() == "30") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.ReserveCapacity:
                    if (stat.Statistics.First().TrimEnd() == "120") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.HeadMultiplier:
                    if (stat.Statistics.First().TrimEnd() == "1.40") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.TorsoMultiplier:
                    if (stat.Statistics.First().TrimEnd() == "1.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.LimbMultiplier:
                    if (stat.Statistics.First().TrimEnd() == "1.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.MuzzleVelocity:
                    if (stat.Statistics.First().TrimEnd() == "2500.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.Suppression:
                    if (stat.Statistics.First().TrimEnd() == "0.50") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.PenetrationDepth:
                    if (stat.Statistics.First().TrimEnd() == "1.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.ReloadTime:
                    if (stat.Statistics.First().TrimEnd() == "2.5") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.EmptyReloadTime:
                    if (stat.Statistics.First().TrimEnd() == "3.2") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.WeaponWalkspeed:
                    if (stat.Statistics.First().TrimEnd() == "14.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.AimingWalkspeed:
                    if (stat.Statistics.First().TrimEnd() == "8.4") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.AmmoType:
                    if (stat.Statistics.First().TrimEnd() == "AMMO TYPE 9.45x39mm") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.SightMagnification:
                    if (stat.Statistics.First().TrimEnd() == "2.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.MinimumTimeToKill:
                    if (stat.Statistics.First().TrimEnd() == "0.20") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.HipfireSpreadFactor:
                    if (stat.Statistics.First().TrimEnd() == "0.05") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.HipfireRecoverySpeed:
                    if (stat.Statistics.First().TrimEnd() == "10.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.HipfireSpreadDamping:
                    if (stat.Statistics.First().TrimEnd() == "0.90") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.HipChoke:
                    if (stat.Statistics.First().TrimEnd() == "0.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.AimChoke:
                    if (stat.Statistics.First().TrimEnd() == "0.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.EquipSpeed:
                    if (stat.Statistics.First().TrimEnd() == "12.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.AimModelSpeed:
                    if (stat.Statistics.First().TrimEnd() == "15.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.AimMagnificationSpeed:
                    if (stat.Statistics.First().TrimEnd() == "12.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.CrosshairSize:
                    if (stat.Statistics.First().TrimEnd() == "30.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.CrosshairSpreadRate:
                    if (stat.Statistics.First().TrimEnd() == "400.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.CrosshairRecoverRate:
                    if (stat.Statistics.First().TrimEnd() == "20.00") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;
                case StatisticOptions.FireModes:
                    if (stat.Statistics.First().TrimEnd() == "AUTO") { score++;
                        PFDBLogger.LogDebug($"{stat.Option} had the correct value.");
                    } else {
                        PFDBLogger.LogWarning($"{stat.Option} did not have the correct value: {stat.Statistics.First().TrimEnd()}");
                    }
                    break;


            }
        }
        //foreach(Statistic a in collection)Console.WriteLine(a.Statistics.First());
        return TestingOutput("FileParse complete test (extracting statistics)", score >= 30, "30", score.ToString());
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
    public static bool TestingOutput(string testName, bool pass, string expectedOutput, string actualOutput, [CallerMemberName] string caller = "") {
        string originalCaller = caller ?? "";
        if (pass) {
            PFDBLogger.LogInformation($"{testName}\u001b[1;32m passed.\u001b[0;0m Expected: {expectedOutput}. Got: {actualOutput}", originalCaller);
            return true;
        } else {
            PFDBLogger.LogError($"{testName}\u001b[1;31m failed.\u001b[0;0m Expected: {expectedOutput}. Got: {actualOutput}", originalCaller);
            return false;
        }
    }

    /// <summary>
    /// Sample text for file imitation (for StatisticParse and FileParse).
    /// </summary>
    public static string sampleText = @"Does the file exist? True 
===== Ballistics ===== 
MINIMUM TIME TO KILL 0.20s 
MUZZLE VELOCITY 2500.00 studs/s
PENETRATION DEPTH 1.00 studs
SUPPRESSION 0.50 
HIPFIRE SPREAD FACTOR 0.05 
===== Accuracies ===== 
HIPFIRE SPREAD FACTOR 0.05 
HIPFIRE RECOVERY SPEED 10.00 
HIPFIRE SPREAD DAMPING 0.90
SIGHT MAGNIFICATION 2.00 
HIP CHOKE 0.00 
AIM CHOKE 0.00 
===== WeaponCharacteristics ===== 
RELOAD TIME 2.5 seconds 
EMPTY RELOAD TIME 3.2 seconds 
EQUIP SPEED 12.00 
AIM MODEL SPEED 15.00 
AIM MAGNIFICATION SPEED 12.00 
CROSSHAIR SIZE 30.00
CROSSHAIR SPREAD RATE 400.00 
CROSSHAIR RECOVER RATE 20.00 
===== Miscellaneous ===== 
WEAPON WALKSPEED 14.00 
AIMING WALKSPEED 8.4 
ROUND IN CHAMBER One 
===== RankInfo ===== 
AN-94
RANK 11
KILLS 2103
OPTICS DEFAULT 
BARREL DEFAULT 
UNDERBARREL DEFAULT 
OTHER DEFAULT 
AMMO DEFAULT 
===== DamageInfo ===== 
30 
rae: 
(0) 
(30) 
(120) 
150 
300 
HEAD MULTIPLIER 1.40 
TORSO MULTIPLIER 1.00 
LIMB MULTIPLIER 1.00 
===== FireInfo ===== 
AMMO CAPAClTY 30/120 

AMMO TYPE 9.45x39mm 
FIRERATE 600A|1800B|600S 
FIRE MODES | AUTO | Il | SEMI |";
}

/*
*/