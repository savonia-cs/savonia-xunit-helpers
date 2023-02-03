﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Savonia.xUnit.Helpers;

// Modelled after <see href="https://andrewlock.net/creating-a-custom-xunit-theory-test-dataattribute-to-load-data-from-json-files/">Creating a custom xUnit theory test DataAttribute to load data from JSON files</see>
// by Andrew Lock and <see href="https://www.ankursheel.com/blog/load-test-data-json-file-xunit-tests">How to load test data from a JSON file for xUnit tests</see>
// by Ankur Sheel.


/// <summary>
/// xUnit theory data provider from json file.
/// Environment variable TEST_DATA_PREFIX value is added to the defined filename when loading the test data file.
///
/// The JSON file or a property in the JSON file is expected to be an object array.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class JsonFileDataAttribute : DataAttribute
{
    /// <summary>
    /// Environment variable name for test data file prefix. The value of this environment variable is added to the specified test data filename before
    /// loading the data. This is used to load different test data file to assignment evaluation tests.
    /// 
    /// If value is not specified to the environment variable then nothing is added to the filename.
    /// </summary>
    public const string EnvVarTestDataPrefix = "TEST_DATA_PREFIX";
    private readonly string _filePath;

    private readonly string? _propertyName;

    private readonly Type _dataType;

    private readonly Type _resultType;

    /// <summary>
    /// Initialize the JSON test data.
    /// Environment variable TEST_DATA_PREFIX value is added to the value of <paramref name="filePath"/> when loading the test data file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="dataType"></param>
    /// <param name="resultType"></param>
    public JsonFileDataAttribute(string filePath, Type dataType, Type resultType)
    {
        _filePath = filePath;
        _dataType = dataType;
        _resultType = resultType;
    }

    /// <summary>
    /// Initialize the JSON test data.
    /// Environment variable TEST_DATA_PREFIX value is added to the value of <paramref name="filePath"/> when loading the test data file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="propertyName"></param>
    /// <param name="dataType"></param>
    /// <param name="resultType"></param>
    public JsonFileDataAttribute(string filePath, string propertyName, Type dataType, Type resultType)
    {
        _filePath = filePath;
        _propertyName = propertyName;
        _dataType = dataType;
        _resultType = resultType;
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

        var parameters = testMethod.GetParameters();

        // Get the absolute path to the JSON file
        var path = Path.IsPathRooted($"{Environment.GetEnvironmentVariable(EnvVarTestDataPrefix)}{_filePath}")
            ? $"{Environment.GetEnvironmentVariable(EnvVarTestDataPrefix)}{_filePath}"
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), $"{Environment.GetEnvironmentVariable(EnvVarTestDataPrefix)}{_filePath}");

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        // Load the file
        var fileData = File.ReadAllText(_filePath);

        if (string.IsNullOrEmpty(_propertyName))
        //whole file is the data
        {
            return GetData(fileData);
        }

        // Only use the specified property as the data
        var allData = JObject.Parse(fileData);
        var data = allData[_propertyName]?.ToString();

        return GetData(data);
    }

    private IEnumerable<object[]> GetData(string? jsonData)
    {
        var objectList = new List<object[]>();
        var specific = typeof(TestObject<,>).MakeGenericType(_dataType, _resultType);
        var generic = typeof(List<>).MakeGenericType(specific);

        if (jsonData != null)
        {
            dynamic? datalist = JsonConvert.DeserializeObject(jsonData, generic);
            if (datalist != null)
            {
                foreach (var data in datalist)
                {
                    if (data != null)
                    {
                        objectList.Add(new object[] { data.Data, data.Result });
                    }
                }
            }
        }

        return objectList;
    }
}
