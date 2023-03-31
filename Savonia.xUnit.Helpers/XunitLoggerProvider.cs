using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Savonia.xUnit.Helpers;

/// <summary>
/// Logger provider for xUnit <see cref="ITestOutputHelper"/>.
/// </summary>
public class XunitLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Create logger provider with <see cref="ITestOutputHelper"/>.
    /// </summary>
    /// <param name="testOutputHelper"></param>
    public XunitLoggerProvider(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Create logger for category
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public ILogger CreateLogger(string categoryName)
        => new XunitLogger(_testOutputHelper, categoryName);

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    { }
}

/// <summary>
/// xUnit logger that uses <see cref="ITestOutputHelper"/>.
/// </summary>
public class XunitLogger : ILogger
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _categoryName;

    /// <summary>
    /// Create xUnit logger with <see cref="ITestOutputHelper"/> and category name.
    /// </summary>
    /// <param name="testOutputHelper"></param>
    /// <param name="categoryName"></param>
    public XunitLogger(ITestOutputHelper testOutputHelper, string categoryName)
    {
        _testOutputHelper = testOutputHelper;
        _categoryName = categoryName;
    }

    /// <summary>
    /// Begin new logging scope
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public IDisposable BeginScope<TState>(TState state)
        => NoopDisposable.Instance;

    /// <summary>
    /// All log levels are enabled.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public bool IsEnabled(LogLevel logLevel)
        => true;

    /// <summary>
    /// Write log entry
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <param name="formatter"></param>
    /// <typeparam name="TState"></typeparam>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        _testOutputHelper.WriteLine($"{_categoryName} [{eventId}] {formatter(state, exception)}");
        if (exception != null)
            _testOutputHelper.WriteLine(exception.ToString());
    }

    private class NoopDisposable : IDisposable
    {
        public static NoopDisposable Instance = new NoopDisposable();
        public void Dispose()
        { }
    }
}
