
using System.IO;
using System.Text;

namespace Savonia.xUnit.Helpers;

/// <summary>
/// Abstract base class for Console App testing.
/// </summary>
public abstract class ConsoleAppTestBase
{
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