namespace Maux;

/// <summary>
/// <see cref="Shell"/> navigation abstraction
/// </summary>
public interface IShellNavigation
{
    /// <summary>
    /// Performs one or multiple relative navigation from the current route
    /// </summary>
    /// <param name="routeChunks"></param>
    /// <returns></returns>
    /// <remarks>
    /// This extension method add support for relative navigation into MAUI Shell.
    /// Parameters can be passed also on intermediate routes but that will cause multiple user visible navigations.
    /// </remarks>
    /// <example>
    /// <code>NavigateAsync("Child", "Nested") // => Child/Nested</code>
    /// <code>NavigateAsync(
    ///     ("Child", new ("childParam1", param1, "childParam2", param2)),
    ///     ("Nested", myIntent)
    /// ) // => Child/Nested</code>
    /// <code>NavigateAsync("..", ("Sibling", siblingParameters)) // => ../Sibling</code>
    /// <code>NavigateAsync("//", "SomeRootPage") // => //SomeRootPage</code>
    /// </example>
    Task NavigateAsync(params ShellNavigationChunk[] routeChunks);
    
    /// <summary>
    /// Performs a navigation to the closest route mapped to the registered <typeparamref name="TPage"/>
    /// within <see cref="MauxMauiAppBuilderExtensions.UseShellNavigation"/>
    /// </summary>
    /// <param name="parameters"></param>
    /// <typeparam name="TPage"></typeparam>
    /// <returns></returns>
    Task NavigateAsync<TPage>(ShellParameters? parameters = null);
    
    /// <summary>
    /// Performs a navigation to the closest route mapped to the registered <typeparamref name="TPage"/>
    /// within <see cref="MauxMauiAppBuilderExtensions.UseShellNavigation"/>
    /// </summary>
    /// <param name="intent"></param>
    /// <typeparam name="TPage"></typeparam>
    /// <returns></returns>
    Task NavigateAsync<TPage>(object intent);
}

/// <inheritdoc cref="IShellNavigation"/>
internal class ShellNavigation : IShellNavigation
{
    private readonly MauxNavigationMap _navigationMap;

    /// <inheritdoc cref="IShellNavigation"/>
    public ShellNavigation(MauxNavigationMap navigationMap)
    {
        _navigationMap = navigationMap;
    }
    
    /// <inheritdoc cref="IShellNavigation.NavigateAsync"/>
    public async Task NavigateAsync(params ShellNavigationChunk[] routeChunks)
    {
        var shell = Shell.Current;
        
        if (routeChunks.Length == 0)
        {
            throw new ArgumentException("At least one route chunk must be specified", nameof(routeChunks));
        }

        var currentRoute = shell.CurrentState.Location.ToString();

        var i = 0;
        ShellNavigationChunk chunk = null!;

        while (i < routeChunks.Length)
        {
            var routeNamesWithoutParameters = new List<string>();
            while (i < routeChunks.Length)
            {
                chunk = routeChunks[i++];
                
                if (chunk.RouteName == "//")
                {
                    currentRoute = "//";
                    continue;
                }
                
                if (chunk.RouteName.StartsWith("//"))
                {
                    currentRoute = "//";
                }
                
                routeNamesWithoutParameters.Add(chunk.RouteName.TrimStart('/'));

                if (chunk.ShellParameters != null) break;
            }

            if (!currentRoute.EndsWith("/"))
            {
                currentRoute += "/";
            }

            currentRoute += string.Join("/", routeNamesWithoutParameters);

            await (chunk.ShellParameters == null
                ? shell.GoToAsync(currentRoute)
                : shell.GoToAsync(currentRoute, chunk.ShellParameters));
        }
    }

    /// <inheritdoc cref="IShellNavigation.NavigateAsync{TPage}(ShellParameters)"/>
    public Task NavigateAsync<TPage>(ShellParameters? parameters = null)
    {
        var shell = Shell.Current;
        var routes = _navigationMap.GetRoutes(typeof(TPage));
        var route = routes.Count == 1
            ? routes.First()
            : GetClosestRoute(shell.CurrentState.Location.ToString(), routes);
        
        return parameters == null ? shell.GoToAsync($"//{route}") : shell.GoToAsync($"//{route}", parameters);
    }

    /// <inheritdoc cref="IShellNavigation.NavigateAsync{TPage}(object)"/>
    public Task NavigateAsync<TPage>(object intent)
        => NavigateAsync<TPage>(new ShellParameters { { MauxIntent.ParameterName, intent } });

    private string GetClosestRoute(string currentRoute, HashSet<string> routes) 
        => routes.OrderByDescending(route => LongestCommonPath(currentRoute, route)).First();

    private int LongestCommonPath(string route1, string route2)
    {
        var segments1 = route1.Split("/");
        var segments2 = route2.Split("/");
        var i = 0;
        while (i < segments1.Length && i < segments2.Length && segments1[i] == segments2[i]) ++i;
        return i;
    }
}