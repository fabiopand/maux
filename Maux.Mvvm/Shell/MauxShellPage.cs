namespace Maux;

/// <summary>
/// A <see cref="T:Microsoft.Maui.Controls.Page" /> that displays a single view.
/// </summary>
/// <remarks>
/// Provides built-in feature to react to application sleep/resume and shell navigation parameters handling via <see cref="MauxIntent"/>.
/// Initializes the view model with the intent provided by the navigation.
/// </remarks>
[PublicAPI]
public abstract class MauxShellPage : MauxContentPage, IQueryAttributable
{
    /// <inheritdoc cref="MauxShellPage"/>
    protected MauxShellPage()
    {
    }

    /// <inheritdoc cref="MauxShellPage"/>
    protected MauxShellPage(IMauxApplication application) : base(application)
    {
    }

    /// <inheritdoc cref="MauxShellPage"/>
    protected MauxShellPage(IMauxApplication application, IMauxPageModel pageModel) : base(application, pageModel)
    {
    }

    /// <inheritdoc cref="IQueryAttributable.ApplyQueryAttributes"/>
    public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(MauxIntent.ParameterName, out var parameterValue))
        {
            Intent = parameterValue;
        }
    }
}