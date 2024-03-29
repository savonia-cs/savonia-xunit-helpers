﻿using System.Text.RegularExpressions;

namespace Savonia.xUnit.Helpers;

/// <summary>
/// Some helper extension methods for string manipulation.
/// </summary>
public static class StringHelpers
{
    /// <summary>
    /// Replaces decimal separators in a string to current culture specification.
    /// </summary>
    /// <param name="str">Source string</param>
    /// <param name="existingSeparator">Decimal separator that exists in source string. These are set to culture specific separator.</param>
    /// <returns></returns>
    public static string SetDecimalSeparator(this string str, string existingSeparator = ".")
    {
        return str.Replace(existingSeparator, System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
    }

    /// <summary>
    /// Replaces new line characters in a string to <see cref="Environment.NewLine"/>.
    /// Note that "\n" characters in text in CSV file is not new line character. Use @"\n" or "\\n" as value for <paramref name="existingNewLine" /> to replace those with 
    /// new line character '\n'. 
    /// </summary>
    /// <param name="str">Source string</param>
    /// <param name="existingNewLine">New line characters that exists in source string. These are set to <see cref="Environment.NewLine"/></param>
    /// <returns></returns>
    public static string SetNewLines(this string str, string existingNewLine = "\n")
    {
        return str.Replace(existingNewLine, Environment.NewLine);
    }

    /// <summary>
    /// Replaces new line characters in a string to \n and removes possible carriage returns (\r).
    /// Note that "\n" characters in text in CSV file is not new line character. Use @"\n" or "\\n" as value for <paramref name="existingNewLine" /> to replace those with 
    /// new line character '\n'. 
    /// </summary>
    /// <param name="str">Source string</param>
    /// <param name="existingNewLine">New line characters that exists in source string. These are set to \n</param>
    /// <returns></returns>
    public static string SetUniversalNewLines(this string str, string existingNewLine = "\n")
    {
        return str.Replace(existingNewLine, "\n").Replace("\r", "");
    }


    /// <summary>
    /// Condense source string. Replaces white spaces " ", carriage return '\r' and new line '\n' with empty string "".
    /// </summary>
    /// <param name="str">String to condense</param>
    /// <returns></returns>
    public static string Condense(this string str)
    {
        return str.Replace(" ", "").Replace("\r", "").Replace("\n", "");
    }

    /// <summary>
    /// Trims (removes) new lines (both possible \r and \n chars) from string end.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string TrimEndNewLines(this string str)
    {
        return str.TrimEnd(Environment.NewLine.ToCharArray());
    }

    /// <summary>
    /// Converts enumerable data to a string where <paramref name="data"/> values are separated by <paramref name="separator"/>.
    /// </summary>
    /// <param name="data">Source data</param>
    /// <param name="separator">Separator in a resulting string</param>
    /// <returns></returns>
    public static string AsString<T>(this IEnumerable<T> data, string separator)
    {
        if (null == data)
        {
            return string.Empty;
        }
        return string.Join(separator, data);
    }

    /// <summary>
    /// Converts enumerable data to a string using <see cref="Environment.NewLine" /> as separator.
    /// Can be used as a standard input to console apps.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string ToStandardInput<T>(this IEnumerable<T> data)
    {
        return data.AsString(Environment.NewLine);
    }

    /// <summary>
    /// Converts params data to a string using <see cref="Environment.NewLine" /> as separator.
    /// Can be used as a standard input to console apps.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string ToStandardInput(params object[] args)
    {
        return args.AsString(Environment.NewLine);
    }

    /// <summary>
    /// Converts a string to lines.
    /// </summary>
    /// <param name="str">Source string to be splitted to lines</param>
    /// <param name="lineSeparator">String to split the lines</param>
    /// <returns></returns>
    public static IEnumerable<string> AsLines(this string str, string lineSeparator = "\n")
    {
        return str.Split(lineSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// Filter out code comments from a string.
    /// </summary>
    /// <param name="str">Source string to remove code comments from</param>
    /// <returns></returns>
    public static string FilterComments(this string str)
    {
        var filters = new List<string> { @"/\*(.*?)\*/", @"//(.*?)\r?\n", @"""((\\[^\n]|[^""\n])*)""", @"@(""[^""]*"")+" };
        string startingBlockComment = "/*"; 
        string startingLineComment = "//";
        string filtered = Regex.Replace(str, string.Join("|", filters),
            me =>
            {
                if (me.Value.StartsWith(startingBlockComment) || me.Value.StartsWith(startingLineComment))
                {
                    return me.Value.StartsWith(startingLineComment) ? Environment.NewLine : "";
                }
                // Keep the literal strings
                return me.Value;
            },
            RegexOptions.Singleline);
        return filtered;
    }

    /// <summary>
    /// Sanitizes <see cref="StringWriter" /> content. Reads the content as string and removes trailing new lines. Then changes remaining new lines 
    /// to '\n' and removes possible carriage returns '\r'.
    /// </summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public static string Sanitize(this StringWriter sw)
    {
        return sw.ToString().TrimEndNewLines().SetUniversalNewLines();
    }

    /// <summary>
    /// Returns true when a string has value. A string containing only white space characters is considered to not have value (i.e. will return false).
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool HasValue(this string str)
    {
        return false == string.IsNullOrWhiteSpace(str);
    }
}
