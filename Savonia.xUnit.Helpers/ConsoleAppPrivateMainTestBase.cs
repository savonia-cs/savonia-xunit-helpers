using System;
using System.Reflection;
using Xunit.Abstractions;

namespace Savonia.xUnit.Helpers;

/// <summary>
/// Abstract base class for Console App testing with private class and private Main method. Uses reflection to execute the Main method.
/// This class inherits <see cref="ConsoleAppTestBase" />.
/// </summary>
public abstract class ConsoleAppPrivateMainTestBase : ConsoleAppTestBase
{
    private object? prog;
    MethodInfo method;

    /// <summary>
    /// Constructor requires the type of the class that contains the Main method to be executed for the tests.
    /// </summary>
    /// <param name="consoleAppType">Type of the class with the Main method</param>
    public ConsoleAppPrivateMainTestBase(Type consoleAppType) : this(consoleAppType, null)
    {
    }

    /// <summary>
    /// Constructor requires the type of the class that contains the Main method to be executed for the tests.
    /// Enables test output capturing via <see cref="ITestOutputHelper" />.
    /// </summary>
    /// <param name="consoleAppType"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    public ConsoleAppPrivateMainTestBase(Type consoleAppType, ITestOutputHelper? output) : base(output)
    {
        Type type = consoleAppType;
        prog = Activator.CreateInstance(type);
        method = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(x => x.Name == "Main" && x.IsStatic).First();
    }
    
    /// <summary>
    /// Execute the Main method with parameters.
    /// </summary>
    /// <param name="args">Parameters to provide to the Main method.</param>
    /// <returns></returns>
    protected object? ExecuteMain(params object[] args)
    {
        return method.Invoke(prog, new object[] { args });
    }

    /// <summary>
    /// Execute the Main method without parameters.
    /// </summary>
    /// <returns></returns>
    protected object? ExecuteMain()
    {
        return method.Invoke(prog, new object[] { });
    }
}