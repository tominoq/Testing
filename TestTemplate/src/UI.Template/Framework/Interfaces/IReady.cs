namespace UI.Template.Framework.Interfaces;

/// <summary>
/// Holds only "Ready" methods to be implemented in page/component types.
/// Each type that needs to have custom <see cref="IsReady"/> and <see cref="WaitForReady"/>, has to implement this interface.
/// </summary>
public interface IReady
{
    /// <summary>
    /// Checks if the page/window/component loading is done
    /// </summary>
    /// <returns>True the page/window/component is doing nothing (based on function content), otherwise false</returns>
    bool IsReady();

    /// <summary>
    /// Waits for the page/window/component to be done loading
    /// </summary>
    void WaitForReady();
}
