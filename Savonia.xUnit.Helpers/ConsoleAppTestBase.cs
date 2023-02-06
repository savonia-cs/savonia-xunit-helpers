
using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace Savonia.xUnit.Helpers;

/// <summary>
/// Abstract base class for Console App testing.
/// </summary>
public abstract class ConsoleAppTestBase
{
    /// <summary>
    /// Use to write test output.
    /// </summary>
    private readonly ITestOutputHelper? _output = null;

    /// <summary>
    /// Default constructor. Does not enable test output capturing.
    /// </summary>
    public ConsoleAppTestBase()
    {
    }

    /// <summary>
    /// Enable test output capturing.
    /// </summary>
    /// <param name="output"></param>
    public ConsoleAppTestBase(ITestOutputHelper output)
    {
        _output = output;
        var testDataPrefix = Environment.GetEnvironmentVariable(JsonFileDataAttribute.EnvVarTestDataPrefix);
        if (false == string.IsNullOrEmpty(testDataPrefix))
        {
            WriteLine($"\n*** Running tests with test data prefix '{testDataPrefix}' ***\n");
        }
    }

    /// <summary>
    /// Write test output via <see cref="ITestOutputHelper" />.
    /// </summary>
    /// <param name="message"></param>
    protected void WriteLine(string message)
    {
        if (null != _output)
        {
            _output.WriteLine(message);
        }
    }

    /// <summary>
    /// Write test output via <see cref="ITestOutputHelper" />.
    /// </summary>
    /// <param name="message"></param>
    protected void WriteLine(string format, params object[] args)
    {
        if (null != _output)
        {
            _output.WriteLine(format, args);
        }
    }

    /// <summary>
    /// Get file content. Returns true and the file's content when the file exists. Return false and null if the file does not exist.
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="encoding">Possible file encoding. Leave to null to automatically detect the encoding from possible Byte-Order Mark (BOM) in the file.</param>
    /// <returns></returns>
    protected (bool fileExists, string? content) GetFileContent(string path, Encoding? encoding = null)
    {
        bool exists = File.Exists(path);
        string? content = null;
        if (exists)
        {
            if (null == encoding)
            {
                content = File.ReadAllText(path);
            }
            else
            {
                content = File.ReadAllText(path, encoding);
            }
        }
        return (exists, content);
    }

    /// <summary>
    /// Set file content. Creates the file or overwrites existing file.
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="content">Content to set to the file</param>
    /// <param name="encoding">Possible file encoding. Leave to null to use UTF-8 without Byte-Order Mark (BOM).</param>
    protected void SetFileContent(string path, string content, Encoding? encoding = null)
    {
        if (null == encoding)
        {
            File.WriteAllText(path, content);
        }
        else
        {
            File.WriteAllText(path, content, encoding);
        }
    }
}