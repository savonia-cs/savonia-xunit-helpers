using Savonia.xUnit.Helpers;
using Xunit.Abstractions;

namespace Savonia.xUnit.Helpers.Tests;

public class TestBaseDataAttributeTests : IDisposable
{
    private readonly ITestOutputHelper? _output = null;
    private readonly string? _existingTestDataPrefix = null;

    public TestBaseDataAttributeTests(ITestOutputHelper output)
    {
        _output = output;
        string? existingTestDataPrefix = Environment.GetEnvironmentVariable(TestBaseDataAttribute.EnvVarTestDataPrefix);
    }

    [Theory]
    [InlineData("testdata.csv", "testdata.csv")]
    [InlineData(@"data\testdata.json", @"data\testdata.json")]
    [InlineData(@"c:\temp\testdata.csv", @"c:\temp\testdata.csv")]
    public void TestGetTestDataFilePathReturnsOriginalFilePath(string filename, string expected)
    {
        // set envvar to null to clear possible existing value
        Environment.SetEnvironmentVariable(TestBaseDataAttribute.EnvVarTestDataPrefix, null);

        TestDataAttribute tester = new TestDataAttribute(filename);
        string filePath = tester.GetFilePath();
        _output.WriteLine($"filepath: {filePath}");

        Assert.Equal(expected, filePath);
    }


    [Theory]
    [InlineData("testdata.csv", "testrunnertestdata.csv")]
    public void TestGetTestDataFilePathReturnsPrefixedFilePathForExistingFiles(string filename, string expected)
    {
        Environment.SetEnvironmentVariable(TestBaseDataAttribute.EnvVarTestDataPrefix, "testrunner");
        var envvar = Environment.GetEnvironmentVariable(TestBaseDataAttribute.EnvVarTestDataPrefix);
        _output.WriteLine($"{TestBaseDataAttribute.EnvVarTestDataPrefix}={envvar}");

        TestDataAttribute tester = new TestDataAttribute(filename);
        string filePath = tester.GetFilePath();
        _output.WriteLine($"filepath: {filePath}");

        Assert.Equal(expected, filePath);
    }

    [Theory]
    [InlineData("testdatanotexitst.csv", "testdatanotexitst.csv")]
    [InlineData(@"data\testdata.json", @"data\testdata.json")]
    [InlineData(@"c:\temp\testdata3215432-is-random-name-and-this-should-not-exist.rand3214", @"c:\temp\testdata3215432-is-random-name-and-this-should-not-exist.rand3214")]
    public void TestGetTestDataFilePathReturnsOriginalFilePathForNonExistingFiles(string filename, string expected)
    {
        Environment.SetEnvironmentVariable(TestBaseDataAttribute.EnvVarTestDataPrefix, "testrunner");
        var envvar = Environment.GetEnvironmentVariable(TestBaseDataAttribute.EnvVarTestDataPrefix);
        _output.WriteLine($"{TestBaseDataAttribute.EnvVarTestDataPrefix}={envvar}");

        TestDataAttribute tester = new TestDataAttribute(filename);
        string filePath = tester.GetFilePath();
        _output.WriteLine($"filepath: {filePath}");

        Assert.Equal(expected, filePath);
    }


    public void Dispose()
    {
        // clear possible set envvar
        Environment.SetEnvironmentVariable(TestBaseDataAttribute.EnvVarTestDataPrefix, _existingTestDataPrefix);
    }

    class TestDataAttribute : TestBaseDataAttribute
    {
        public TestDataAttribute(string filename) : base(filename)
        {}

        public string GetFilePath()
        {
            return base.GetTestDataFilePath();
        }
    }
}