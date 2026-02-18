using UI.Template.Framework.Logging;

namespace UI.Template.Tests;

/// <summary>
/// The <see cref="SetUpProject" /> class whose methods will be executed once
/// before and after any of the fixtures contained in its namespace.
/// </summary>
[SetUpFixture]
public abstract class SetUpProject
{
    /// <summary>
    /// Initializes the test project.
    /// </summary>
    [OneTimeSetUp]
    public void ProjectOneTimeSetUp()
    {
        LogFactory.InitializeTestProjectLogger();
    }

    /// <summary>
    /// Finalizes the test project.
    /// </summary>
    [OneTimeTearDown]
    public void ProjectOneTimeTearDown()
    {
        LogFactory.CloseLogger();
    }
}
