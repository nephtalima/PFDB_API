using System;

namespace PFDB.Parsing;

/// <summary>
/// Specifies the defaults for StatisticParse and related dependencies.
/// </summary>
public static class DefaultStatisticParameters
{


    /// <summary>
    /// Specifies the acceptable number spaces between both words. Default is set to 3.
    /// </summary>
    public static int AcceptableSpaces = 3;

    /// <summary>
    /// Specifies the acceptable number spaces that a corrupted word can have. Default is set to 3.
    /// </summary>
    public static int AcceptableCorruptedWordSpaces = 3;

    /// <summary>
    /// Specifies the StringComparison method to be used. Default is InvariantCultureIgnoreCase.
    /// </summary>
    public static StringComparison StringComparisonMethod = StringComparison.InvariantCultureIgnoreCase;

    
}