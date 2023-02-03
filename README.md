# Savonia.xUnit.Helpers

This package contains helper classes for xUnit tests mainly used to automatically evaluate and assess C# assignments.

Contains

- CSV data provider attribute to read theory test cases from a CSV file.
- JSON data provider attribure to read theory test cases from a JSON file.
- Abstract base classes for Console App testing and source code reading.
- Helper methods for string manipulation.

CSV and JSON data providers reads environment variable named TEST_DATA_PREFIX and adds the prefix value to the provided test data filename before loading the data. This is used to run the tests with different data for assignment evaluation.