# Savonia.xUnit.Helpers

This package contains helper classes for xUnit tests mainly used to automatically evaluate and assess C# assignments.

Contains

- CSV data provider attribute to read theory test cases from a CSV file.
- JSON data provider attribute to read theory test cases from a JSON file.
- Abstract base classes for Console App testing and source code reading.
- Helper methods for string manipulation.
- Helper methods for HTML and HtmlClient for UI testing.

CSV and JSON data providers reads environment variable named TEST_DATA_PREFIX and adds the prefix value to the provided test data filename before loading the data. This is used to run the tests with different data for assignment evaluation.

## Note about web testing

This package references specific version of AngleSharp to be compatible with [bunit](https://www.nuget.org/packages/bunit). bunit depends on AngleSharp.Diffing which in turn depends on [AngleSharp.Css](https://www.nuget.org/packages/AngleSharp.Css) >= 1.0.0-beta.144 which has dependency on AngleSharp >= 1.0.0 and < 2.0.0.