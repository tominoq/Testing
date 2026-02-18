using NUnit.Framework.Interfaces;

namespace UI.Template.NUnit;

/// <summary>
/// Information about the current test.
/// </summary>
public static class TestInfo
{
    /// <summary>
    /// Gets the arguments of the current test.
    /// </summary>
    public static IReadOnlyCollection<object?> Arguments => Array.AsReadOnly(TestContext.CurrentContext.Test.Arguments);

    /// <summary>
    /// Gets the name of the current test class.
    /// </summary>
    public static string? ClassName => TestContext.CurrentContext.Test.ClassName?.Split(".").Last();

    /// <summary>
    /// Gets the full name of the current test class/method.
    /// </summary>
    public static string FullName => Name is null ? TestContext.CurrentContext.Test.FullName : $"{Namespace}.{ClassName}.{Name}";

    /// <summary>
    /// Gets the unique identifier of the current test.
    /// </summary>
    public static string Id => TestContext.CurrentContext.Test.ID;

    /// <summary>
    /// Gets the name of the current test.
    /// </summary>
    /// <remarks>
    /// Is empty in the scope of the test class (e.g. in OneTimeSetUp or OneTimeTearDown methods).
    /// </remarks>
    public static string? Name => TestContext.CurrentContext.Test.MethodName;

    /// <summary>
    /// Gets the namespace of the current test.
    /// </summary>
    public static string? Namespace => TestContext.CurrentContext.Test.Namespace;

    /// <summary>
    /// Gets the properties of the current test.
    /// </summary>
    /// <note>
    /// The properties from all levels are combined and sorted by their names.
    /// </note>
    public static IDictionary<string, IReadOnlyCollection<object>> Properties
    {
        get
        {
            var properties = new Dictionary<string, List<object>>();

            foreach (var property in TestContext.CurrentContext.Test.PropertyHierarchy())
            {
                if (!properties.TryGetValue(property.Key.Name, out List<object>? value))
                {
                    value = [];
                    properties.Add(property.Key.Name, value);
                }

                value.AddRange(property.Value.Cast<object>());
            }

            return properties.OrderBy(p => p.Key)
                             .ToDictionary(p => p.Key, p => (IReadOnlyCollection<object>)p.Value.AsReadOnly());
        }
    }

    /// <summary>
    /// Gets the result of the current test.
    /// </summary>
    public static TestStatus Outcome => Result.Outcome.Status;

    /// <summary>
    /// Gets the <see cref="TestContext.ResultAdapter" /> of the current test.
    /// </summary>
    public static TestContext.ResultAdapter Result => TestContext.CurrentContext.Result;

    /// <summary>
    /// Gets the worker name of the current test.
    /// </summary>
    public static string? WorkerName => TestContext.CurrentContext.WorkerId;
}
