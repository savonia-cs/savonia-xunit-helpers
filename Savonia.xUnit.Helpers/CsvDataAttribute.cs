using System.Data;
using System.Reflection;
using NReco.Csv;
using Xunit.Sdk;
using System.Globalization;

namespace Savonia.xUnit.Helpers;

/// <summary>
/// xUnit theory data provider from csv file.
/// Environment variable TEST_DATA_PREFIX value is added to the defined filename when loading the test data file.
/// 
/// For numeric values in the CSV file use invariant culture format (e.g. dot (.) as decimal separator like 3.14)
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class CsvDataAttribute : TestBaseDataAttribute
{
    private readonly string _delimeter = ",";
    private readonly bool _hasHeader = true;

    /// <summary>
    /// Constructor to initialize the test with CSV test data containing comma (,) separated values with header record.
    /// Environment variable TEST_DATA_PREFIX value is added to the value of <paramref name="fileName"/> when loading the test data file.
    /// 
    /// For numeric values in the CSV file use invariant culture format (e.g. dot (.) as decimal separator like 3.14)
    /// </summary>
    /// <param name="fileName"></param>
    public CsvDataAttribute(string fileName) : base(fileName)
    {
    }

    /// <summary>
    /// Constructor to initialize the test with CSV test data containing comma (,) separated values and header record info.
    /// Environment variable TEST_DATA_PREFIX value is added to the value of <paramref name="fileName"/> when loading the test data file.
    /// 
    /// For numeric values in the CSV file use invariant culture format (e.g. dot (.) as decimal separator like 3.14)
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="hasHeaderRow"></param>
    public CsvDataAttribute(string fileName, bool hasHeaderRow) : base(fileName)
    {
        _hasHeader = hasHeaderRow;
    }

    /// <summary>
    /// Constructor to initialize the test with CSV test data, data delimeter and header record info.
    /// Environment variable TEST_DATA_PREFIX value is added to the value of <paramref name="fileName"/> when loading the test data file.
    /// 
    /// For numeric values in the CSV file use invariant culture format (e.g. dot (.) as decimal separator like 3.14)
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="delimeter"></param>
    /// <param name="hasHeaderRow"></param>
    public CsvDataAttribute(string fileName, string delimeter, bool hasHeaderRow) : base(fileName)
    {
        _delimeter = delimeter;
        _hasHeader = hasHeaderRow;
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
        return GetData(GetTestDataFilePath(), pars.Select(par => par.ParameterType).ToArray());
    }

    private IEnumerable<object[]> GetData(string fileName, Type[] parameterTypes)
    {
        if (false == System.IO.File.Exists(fileName))
        {
            throw new FileNotFoundException(fileName);
        }
        
        using (var sr = new StreamReader(File.OpenRead(fileName)))
        {
            var csvReader = new CsvReader(sr, _delimeter);
            if (_hasHeader)
            {
                // skip header row
                csvReader.Read();
            }
            while (csvReader.Read())
            {
                string[] row = new string[csvReader.FieldsCount];                
                for (int i = 0; i < csvReader.FieldsCount; i++)
                {
                    row[i] = csvReader[i];
                }
                yield return ConvertParameters(row, parameterTypes);
            }
        }
    }

    private static object[] ConvertParameters(string[] values, Type[] parameterTypes)
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
    /// <param name="type">The destination parameter type (null if not known)</param>
    /// <returns>The converted parameter value</returns>
    private static object ConvertParameter(string parameter, Type? type)
    {
        if (null == type)
        {
            return parameter;
        }
        if (type.Equals(typeof(string)))
        {
            return parameter;
        }
        else if (type.Equals(typeof(int)) || type.Equals(typeof(int?)))
        {
            if (int.TryParse(parameter, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                return value;
        }
        else if (type.Equals(typeof(double)) || type.Equals(typeof(double?)))
        {
            if (double.TryParse(parameter, NumberStyles.Number, CultureInfo.InvariantCulture, out double value))
                return value;
        }
        else if (type.Equals(typeof(float)) || type.Equals(typeof(float?)))
        {
            if (float.TryParse(parameter, NumberStyles.Number, CultureInfo.InvariantCulture, out float value))
                return value;
        }
        else if (type.Equals(typeof(long)) || type.Equals(typeof(long?)))
        {
            if (long.TryParse(parameter, NumberStyles.Integer, CultureInfo.InvariantCulture, out long value))
                return value;
        }
        else if (type.Equals(typeof(decimal)) || type.Equals(typeof(decimal?)))
        {
            if (decimal.TryParse(parameter, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
                return value;
        }
        else if (type.Equals(typeof(bool)) || type.Equals(typeof(bool?)))
        {
            if (bool.TryParse(parameter, out bool value))
                return value;
        }

        return parameter;
    }
}