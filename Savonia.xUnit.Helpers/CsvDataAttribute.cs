using System.Data;
using System.Reflection;
using Xunit.Sdk;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Savonia.xUnit.Helpers;

/// <summary>
/// xUnit theory data provider from csv file.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class CsvDataAttribute : DataAttribute
{
    /// <summary>
    /// Test data file name
    /// </summary>
    /// <value></value>
    public string FileName { get; private set; }
    CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = ";",
        HasHeaderRecord = true,
        Quote = '"',
    };

    /// <summary>
    /// Default constructor to initialize the test with CSV test data.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="delimeter"></param>
    /// <param name="hasHeaderRow"></param>
    public CsvDataAttribute(string fileName, string delimeter, bool hasHeaderRow)
    {
        FileName = fileName;
        config.Delimiter = delimeter;
        config.HasHeaderRecord = hasHeaderRow;
    }

    /// <summary>
    /// The test uses GetData method to read the test data
    /// </summary>
    /// <param name="testMethod"></param>
    /// <returns></returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (testMethod == null)
        {
            throw new ArgumentNullException(nameof(testMethod));
        }

        ParameterInfo[] pars = testMethod.GetParameters();
        return DataSource(FileName, pars.Select(par => par.ParameterType).ToArray());
    }

    private IEnumerable<object[]> DataSource(string fileName, Type[] parameterTypes)
    {
        if (false == System.IO.File.Exists(fileName))
        {
            throw new FileNotFoundException(fileName);
        }
        using (var reader = new StreamReader(fileName))
        using (var csv = new CsvReader(reader, config))
        {
            // Do any configuration to `CsvReader` before creating CsvDataReader.
            using (var dr = new CsvDataReader(csv))
            {
                var dt = new DataTable();
                dt.Load(dr);

                foreach (DataRow row in dt.Rows)
                {
                    yield return ConvertParameters(row.ItemArray, parameterTypes);
                }
            }
        }
    }

    private static string GetFullFilename(string filename)
    {
        string executable = new Uri(Assembly.GetExecutingAssembly().Location).LocalPath;
        return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(executable), filename));
    }

    private static object[] ConvertParameters(object[] values, Type[] parameterTypes)
    {
        object[] result = new object[values.Length];

        for (int idx = 0; idx < values.Length; idx++)
            result[idx] = ConvertParameter(values[idx], idx >= parameterTypes.Length ? null : parameterTypes[idx]);

        return result;
    }

    /// <summary>
    /// Converts a parameter to its destination parameter type, if necessary.
    /// </summary>
    /// <param name="parameter">The parameter value</param>
    /// <param name="parameterType">The destination parameter type (null if not known)</param>
    /// <returns>The converted parameter value</returns>
    private static object ConvertParameter(object parameter, Type parameterType)
    {
        if ((parameter is double || parameter is float) &&
            (parameterType == typeof(int) || parameterType == typeof(int?)))
        {
            int intValue;
            string floatValueAsString = parameter.ToString();

            if (Int32.TryParse(floatValueAsString, out intValue))
                return intValue;
        }

        return parameter;
    }
}