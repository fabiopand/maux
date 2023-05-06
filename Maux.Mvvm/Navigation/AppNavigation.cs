namespace Maux;

/// <summary>
/// Provides navigation APIs based on <see cref="NavigationPage"/>.
/// </summary>
public interface IAppNavigation
{
    /// <inheritdoc cref="INavigation.ModalStack"/>
    IReadOnlyList<Page> ModalStack { get; }

    /// <inheritdoc cref="INavigation.NavigationStack"/>
    IReadOnlyList<Page> NavigationStack { get; }
    
    /// <inheritdoc cref="INavigation.RemovePage"/>
    void RemovePage(Page page);
    
    /// <summary>
    /// Pushes a page onto the navigation stack asynchronously
    /// </summary>
    /// <param name="intent">
    /// If the target page is a <see cref="MauxContentPage"/> sets the intent
    /// which will be passed to <see cref="MauxPageModel"/>.<see cref="MauxPageModel.PrepareAsync"/>.
    /// </param>
    /// <param name="animated"></param>
    /// <typeparam name="TPage"></typeparam>
    /// <returns></returns>
    /// <remarks>
    /// Page will be created in a <see cref="ServiceLifetime.Scoped"/> service lifetime
    /// that lasts until page is removed from the navigation stack.
    /// </remarks>
    Task PushAsync<TPage>(object? intent = null, bool animated = true) where TPage : Page;
    
    /// <summary>
    /// Pushes a page onto the modal stack asynchronously
    /// </summary>
    /// <param name="intent">
    /// If the target page is a <see cref="MauxContentPage"/> sets the intent
    /// which will be passed to <see cref="MauxPageModel"/>.<see cref="MauxPageModel.PrepareAsync"/>.
    /// </param>
    /// <param name="animated"></param>
    /// <typeparam name="TPage"></typeparam>
    /// <returns></returns>
    /// <remarks>
    /// Page will be created in a <see cref="ServiceLifetime.Scoped"/> service lifetime
    /// that lasts until page is removed from the navigation stack.
    /// </remarks>
    Task PushModalAsync<TPage>(object? intent = null, bool animated = true) where TPage : Page;

    /// <summary>
    /// Pops the current page off the navigation stack asynchronously
    /// </summary>
    /// <param name="intent">
    /// If the target page is a <see cref="MauxContentPage"/> sets the intent
    /// which will be passed to <see cref="MauxPageModel"/>.<see cref="MauxPageModel.PrepareAsync"/>.
    /// </param>
    /// <param name="animated"></param>
    /// <returns></returns>
    Task PopAsync(object? intent = null, bool animated = true);
    
    /// <summary>
    /// Pops the current page off the modal stack asynchronously
    /// </summary>
    /// <param name="intent">
    /// If the target page is a <see cref="MauxContentPage"/> sets the intent
    /// which will be passed to <see cref="MauxPageModel"/>.<see cref="MauxPageModel.PrepareAsync"/>.
    /// </param>
    /// <param name="animated"></param>
    /// <returns></returns>
    Task PopModalAsync(object? intent = null, bool animated = true);
    
    /// <summary>
    /// Pops all but the root page off the navigation stack asynchronously
    /// </summary>
    /// <param name="intent">
    /// If the root page is a <see cref="MauxContentPage"/> sets the intent
    /// which will be passed to <see cref="MauxPageModel"/>.<see cref="MauxPageModel.PrepareAsync"/>.
    /// </param>
    /// <param name="animated"></param>
    /// <returns></returns>
    Task PopToRootAsync(object? intent = null, bool animated = true);

    /// <summary>
    /// Inserts a page before a page in the navigation stack
    /// </summary>
    /// <param name="before">The page will be inserted before this one in the navigation stack.</param>
    /// <param name="intent">
    /// If the target page is a <see cref="MauxContentPage"/> sets the intent
    /// which will be passed to <see cref="MauxPageModel"/>.<see cref="MauxPageModel.PrepareAsync"/>.
    /// </param>
    /// <typeparam name="TPage"></typeparam>
    /// <returns></returns>
    /// <remarks>
    /// Page will be created in a <see cref="ServiceLifetime.Scoped"/> service lifetime
    /// that lasts until page is removed from the navigation stack.
    /// </remarks>
    void InsertPageBefore<TPage>(Page before, object? intent = null) where TPage : Page;
}

internal interface IAppNavigationSetter
{
    void SetCurrentNavigation(INavigation? navigation);
}

internal class AppNavigation : IAppNavigation, IAppNavigationSetter
{
    private readonly IServiceProvider _serviceProvider;
    private INavigation? _navigation;

    public AppNavigation(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IReadOnlyList<Page> ModalStack { 
        get {
            ThrowIfNoNavigation();
            return _navigation!.ModalStack;
        }
    }
    
    public IReadOnlyList<Page> NavigationStack { 
        get {
            ThrowIfNoNavigation();
            return _navigation!.NavigationStack;
        }
    }

    public void RemovePage(Page page)
    {
        ThrowIfNoNavigation();
        _navigation!.RemovePage(page);
    }

    public Task PushAsync<TPage>(object? intent = null, bool animated = true) where TPage : Page
    {
        ThrowIfNoNavigation();
        
        var factory = ScopedRouteFactory<TPage>.Instance;

        var page = (Page)factory.GetOrCreate(_serviceProvider);
        if (intent != null && page is MauxContentPage mauxPage)
        {
            mauxPage.Intent = intent;
        }
        return _navigation!.PushAsync(page, animated);
    }
    
    public Task PushModalAsync<TPage>(object? intent = null, bool animated = true) where TPage : Page
    {
        ThrowIfNoNavigation();
        
        var factory = ScopedRouteFactory<TPage>.Instance;

        var page = (Page)factory.GetOrCreate(_serviceProvider);
        if (intent != null && page is MauxContentPage mauxPage)
        {
            mauxPage.Intent = intent;
        }
        return _navigation!.PushModalAsync(page, animated);
    }
    
    public void InsertPageBefore<TPage>(Page before, object? intent = null) where TPage : Page
    {
        ThrowIfNoNavigation();
        
        var factory = ScopedRouteFactory<TPage>.Instance;

        var page = (Page)factory.GetOrCreate(_serviceProvider);
        if (intent != null && page is MauxContentPage mauxPage)
        {
            mauxPage.Intent = intent;
        }
        
        _navigation!.InsertPageBefore(page, before);
    }

    public Task PopAsync(object? intent = null, bool animated = true)
    {
        ThrowIfNoNavigation();

        var navigationStack = _navigation!.NavigationStack;
        if (intent != null && navigationStack.Count > 1 && navigationStack[^2] is MauxContentPage mauxPage)
        {
            mauxPage.Intent = intent;
        }
        
        return _navigation!.PopAsync(animated);
    }
    
    public Task PopModalAsync(object? intent = null, bool animated = true)
    {
        ThrowIfNoNavigation();

        var modalStack = _navigation!.ModalStack;
        var navigationStack = _navigation!.NavigationStack;
        
        if (intent != null)
        {
            if (modalStack.Count > 1)
            {
                if (modalStack[^2] is MauxContentPage mauxPage)
                {
                    mauxPage.Intent = intent;
                }
            }
            else if (navigationStack.Count > 0 && navigationStack[^1] is MauxContentPage mauxPage)
            {
                mauxPage.Intent = intent;
            }
        }
        
        return _navigation!.PopModalAsync(animated);
    }

    public Task PopToRootAsync(object? intent = null, bool animated = true)
    {
        ThrowIfNoNavigation();

        var navigationStack = _navigation!.NavigationStack;
        if (intent != null && navigationStack.Count > 1 && navigationStack[0] is MauxContentPage mauxPage)
        {
            mauxPage.Intent = intent;
        }
        
        return _navigation!.PopToRootAsync(animated);
    }

    void IAppNavigationSetter.SetCurrentNavigation(INavigation? navigation)
    {
        _navigation = navigation;
    }
    
    private void ThrowIfNoNavigation()
    {
        if (_navigation == null)
        {
            throw new InvalidOperationException("Current INavigation is null");
        }
    }

}