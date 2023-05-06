namespace Maux;

/// <summary>
/// Decide when <see cref="MauxContentPage.OnApplicationEnteringSleep"/> and <see cref="MauxContentPage.OnApplicationResuming"/> are invoked.
/// By default those callback are invoked only when the page is the one currently visible.
/// </summary>
public enum PageSleepBehavior
{
    /// <summary>
    /// Invoked only if this is the visible page
    /// </summary>
    VisibleOnly,

    /// <summary>
    /// Invoked always, also after navigating to nested pages
    /// </summary>
    Lifetime
}

/// <summary>
/// A <see cref="T:Microsoft.Maui.Controls.Page" /> that displays a single view.
/// </summary>
/// <remarks>
/// Provides built-in feature to react to application sleep/resume.
/// </remarks>
[PublicAPI]
public class MauxContentPage : ContentPage, IDisposable
{
    private readonly IMauxApplication _application;
    private bool _appeared;
    private bool _intentChanged;
    private object? _intent;
    private readonly IAppNavigationSetter? _navigation;

    /// <summary>
    /// Stores the latest intent provided to the page
    /// </summary>
    /// <remarks>
    /// When intent changes, <see cref="IMauxPageModel.OnPrepareAsync"/> is invoked.
    /// </remarks>
    internal object? Intent
    {
        get => _intent;
        set
        {
            _intentChanged = true;
            _intent = value;
        }
    }

    /// <inheritdoc cref="PageSleepBehavior"/>
    protected PageSleepBehavior SleepBehavior { get; set; } = PageSleepBehavior.VisibleOnly;

    /// <summary>
    /// Creates a new content page without a page model
    /// </summary>
    /// <remarks>
    /// <see cref="IMauxApplication"/> is resolved from <see cref="Application.Current"/>.
    /// </remarks>
    protected MauxContentPage() : this((IMauxApplication)Application.Current!, null) { }
    
    /// <summary>
    /// Creates a new content page without a page model
    /// </summary>
    /// <param name="application"></param>
    // ReSharper disable once IntroduceOptionalParameters.Global
    protected MauxContentPage(IMauxApplication application) : this(application, null) { }

    /// <summary>
    /// Creates a new content page with a page model and optionally an intent
    /// </summary>
    /// <param name="application"></param>
    /// <param name="pageModel"></param>
    /// <param name="intent"></param>
    protected MauxContentPage(IMauxApplication application, IMauxPageModel? pageModel, object? intent = null)
    {
        _application = application;
        _navigation = application.Services.GetService<IAppNavigationSetter>();
        
        Intent = intent;

        application.EnteringSleep += ApplicationEnteringSleepHandler;
        application.Resuming += ApplicationResumingHandler;

        if (pageModel != null)
        {
            BindingContext = pageModel;
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Page.OnAppearing()"/>
    protected override async void OnAppearing()
    {
        _appeared = true;
        _navigation?.SetCurrentNavigation(Navigation);
        
        base.OnAppearing();

        if (_intentChanged && BindingContext is IMauxPageModel pageModel)
        {
            _intentChanged = false;
            await pageModel.OnPrepareAsync(Intent);
        }
    }

    /// <inheritdoc cref="Page.OnDisappearing()"/>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _appeared = false;
    }

    /// <summary>
    /// This method will run after that the application resumes from sleep mode.
    /// </summary>
    protected virtual void OnApplicationResuming()
    {
    }

    /// <summary>
    /// This method will run when the application is entering into sleep mode.
    /// </summary>
    protected virtual void OnApplicationEnteringSleep()
    {

    }

    /// <summary>
    /// Disposes the content page by unsubscribing from application events
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _application.EnteringSleep -= ApplicationEnteringSleepHandler;
            _application.Resuming -= ApplicationResumingHandler;
        }
    }

    private void ApplicationResumingHandler(object? sender, EventArgs args)
    {
        if (SleepBehavior == PageSleepBehavior.Lifetime || _appeared)
        {
            OnApplicationResuming();
        }
    }

    private void ApplicationEnteringSleepHandler(object? sender, EventArgs args)
    {
        if (SleepBehavior == PageSleepBehavior.Lifetime || _appeared)
        {
            OnApplicationEnteringSleep();
        }
    }
}