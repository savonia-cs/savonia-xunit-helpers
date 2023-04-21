using System.Reflection;
using Xunit.Sdk;

namespace Savonia.xUnit.Helpers;

/// <summary>
/// xUnit theory data provider base class.
/// Handles Environment variable TEST_DATA_PREFIX value addition to test data filename.
/// </summary>
public abstract class TestBaseDataAttribute : DataAttribute
{
    /// <summary>
    /// Environment variable name for test data file prefix. The value of this environment variable is added to the specified test data filename before
    /// loading the data. This is used to load different test data file to assignment evaluation tests.
    /// 
    /// If value is not specified to the environment variable then nothing is added to the filename.
    /// </summary>
    public const string EnvVarTestDataPrefix = "TEST_DATA_PREFIX";

    private readonly string _filename;

    /// <summary>
    /// Provide test data filename.
    /// </summary>
    /// <param name="filename"></param>
    public TestBaseDataAttribute(string filename)
    {
        _filename = filename;
    }

    /// <summary>
    /// Get test data file path. When environment variable TEST_DATA_PREFIX has value it is added to the test data filename and checked if the resulting test data file exists.
    /// If it exists then that file path is returned. If the prefixed test data file does not exist then the original file's path is returned.
    /// </summary>
    /// <value></value>
    protected string GetTestDataFilePath() 
    { 
        string filePath = GetFilePath(_filename);
        var testDataPrefix = Environment.GetEnvironmentVariable(EnvVarTestDataPrefix);
        if (false == string.IsNullOrWhiteSpace(testDataPrefix))
        {
            // env has value
            string prefixedFilePath = Path.Combine(Path.GetDirectoryName(filePath)!, $"{testDataPrefix}{Path.GetFileName(filePath)}");
            if (File.Exists(prefixedFilePath))
            {
                return prefixedFilePath;
            }
        }
        return filePath;
    }

    private string GetFilePath(string filename)
    {
        var path = Path.IsPathRooted($"{filename}")
                    ? $"{filename}"
                    : Path.GetRelativePath(Directory.GetCurrentDirectory(), $"{filename}");
        return path;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="testMethod"></param>
    /// <returns></returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        return null;
    }
}
