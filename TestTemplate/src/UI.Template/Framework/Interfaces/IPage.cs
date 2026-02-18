namespace UI.Template.Framework.Interfaces;

/// <summary>
/// Interface of common logic for working with Page objects.
/// </summary>
public interface IPage : IBase, IReady
{
    /// <summary>
    /// Opens the page in the browser.
    /// </summary>
    void Open();

    /// <summary>
    /// Go back to a previous page in the browser.
    /// </summary>
    void Back();

    /// <summary>
    /// Refreshes the page.
    /// </summary>
    void Refresh()
    {
        Globals.Refresh();
    }

    /// <summary>
    /// Method closes the window/tab and automatically switch to main window/tab.
    /// </summary>
    void Close();

    /// <summary>
    /// Set Url and Path properties of the page class. It doesn't do any WebDriver action (navigating to the Url etc.).
    /// </summary>
    /// <param name="path">valid path string to be set as path and url</param>
    /// <exception cref="ArgumentException">when invalid url string is passed in</exception>
    void SetUrl(string path);

    /// <summary>
    /// Go back to a previous page in the browser and return new page object.
    /// </summary>
    T Back<T>() where T : IPage
    {
        Back();
        T page = (T)(Activator.CreateInstance(typeof(T)) ?? throw new InvalidOperationException($"Could not create an instance of type {typeof(T).FullName}."));
        return page;
    }
}
