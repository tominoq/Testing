using UI.Template.Components.Containers;

namespace UI.Template.Pages;

/// <summary>
/// Base class for all e-shop pages.
/// </summary>
/// <param name="url"></param>
public abstract class BaseEshopPage(string url) : BasePage(url)
{
    public HeaderContainer Header { get; private set; } = new();

    /// <inheritdoc/>
    public override bool IsReady()
    {
        return base.IsReady() && Header.IsDisplayed();
    }
}
