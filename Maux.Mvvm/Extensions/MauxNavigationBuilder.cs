using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Maux;

internal class MauxNavigationMap
{
    // ReSharper disable once CollectionNeverQueried.Local
    private readonly Dictionary<string, Type> _routeToType = new();
    private readonly Dictionary<Type, HashSet<string>> _typeToRoute = new();

    public void Add(string route, Type type)
    {
        _routeToType.Add(route, type);
        if (_typeToRoute.TryGetValue(type, out var routes))
        {
            routes.Add(route);
        }
        else
        {
            _typeToRoute[type] = new HashSet<string> { route };
        }
    }

    public Type GetRouteType(string route)
    {
        return _routeToType[route];
    }

    public HashSet<string> GetRoutes(Type type)
    {
        if (_typeToRoute.TryGetValue(type, out var routes))
        {
            return routes;
        }

        throw new KeyNotFoundException($"No registered routes found for {type.FullName}");
    }
}

/// <summary>
/// Fluently add shell navigation routes to DI
/// </summary>
[PublicAPI]
public class MauxNavigationBuilder
{
    private readonly IServiceCollection _services;
    private readonly MauxNavigationMap _navigationMap;
    private readonly string _baseRoute;

    internal MauxNavigationBuilder(IServiceCollection services, MauxNavigationMap navigationMap, string baseRoute)
    {
        _services = services;
        _navigationMap = navigationMap;
        _baseRoute = baseRoute;
    }

    /// <summary>
    /// Adds a <see cref="Page"/> of the type specified in <typeparamref name="TPage"/>
    /// to the specified <see cref="IServiceCollection"/> with <see cref="ServiceLifetime.Scoped"/> lifetime.
    /// This element will be usable via <see cref="ShellContent"/> setting <see cref="ShellContent.ContentTemplate"/> via
    /// <see cref="ShellContentTemplateExtension"/>.
    /// A <see cref="IServiceScope"/> will be created upon route navigation and disposed on element <see cref="Page.Unloaded"/>.
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="navigation"></param>
    /// <returns></returns>
    public MauxNavigationBuilder AddContent<TPage>(Action<MauxNavigationBuilder>? navigation = null)
        where TPage : Page
        => AddContent<TPage>(typeof(TPage).Name, navigation);

    /// <summary>
    /// Adds a <see cref="Page"/> of the type specified in <typeparamref name="TPage"/>
    /// to the specified <see cref="IServiceCollection"/> with <see cref="ServiceLifetime.Scoped"/> lifetime.
    /// This element will be usable via <see cref="ShellContent"/> setting <see cref="ShellContent.ContentTemplate"/> via
    /// <see cref="ShellContentTemplateExtension"/>.
    /// A <see cref="IServiceScope"/> will be created upon route navigation and disposed on element <see cref="Page.Unloaded"/>.
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="route"></param>
    /// <param name="navigation"></param>
    /// <returns></returns>
    public MauxNavigationBuilder AddContent<TPage>(string route, Action<MauxNavigationBuilder>? navigation = null)
        where TPage : Page
    {
        _services.TryAddScoped<TPage>();

        var elementRoute = $"{_baseRoute}/{route}".TrimStart('/');
        _navigationMap.Add(elementRoute, typeof(TPage));

        navigation?.Invoke(new MauxNavigationBuilder(_services, _navigationMap, elementRoute));
        return this;
    }

    /// <summary>
    /// Adds a <see cref="Page"/> of the type specified in <typeparamref name="TPage"/> and a ViewModel
    /// of the type specified in <typeparamref name="TViewModel"/> to the specified
    /// <see cref="IServiceCollection"/> with <see cref="ServiceLifetime.Scoped"/> lifetime.
    /// This element will be usable via <see cref="ShellContent"/> setting <see cref="ShellContent.ContentTemplate"/> via
    /// <see cref="ShellContentTemplateExtension"/>.
    /// A <see cref="IServiceScope"/> will be created upon route navigation and disposed on element <see cref="Page.Unloaded"/>.
    /// </summary>
    /// <remarks>
    /// Developers are still responsible for assigning the injected instance of <typeparamref name="TViewModel" /> 
    /// to the BindingContext property of <typeparamref name="TPage" />.
    /// </remarks>
    /// <typeparam name="TPage"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="navigation"></param>
    /// <returns></returns>
    public MauxNavigationBuilder AddContent<TPage, TViewModel>(Action<MauxNavigationBuilder>? navigation = null)
        where TPage : Page
        where TViewModel : class, INotifyPropertyChanged
        => AddContent<TPage, TViewModel>(typeof(TPage).Name, navigation);

    /// <summary>
    /// Adds a <see cref="Page"/> of the type specified in <typeparamref name="TPage"/> and a ViewModel
    /// of the type specified in <typeparamref name="TViewModel"/> to the specified
    /// <see cref="IServiceCollection"/> with <see cref="ServiceLifetime.Scoped"/> lifetime.
    /// This element will be usable via <see cref="ShellContent"/> setting <see cref="ShellContent.ContentTemplate"/> via
    /// <see cref="ShellContentTemplateExtension"/>.
    /// A <see cref="IServiceScope"/> will be created upon route navigation and disposed on element <see cref="Page.Unloaded"/>.
    /// </summary>
    /// <remarks>
    /// Developers are still responsible for assigning the injected instance of <typeparamref name="TViewModel" /> 
    /// to the BindingContext property of <typeparamref name="TPage" />.
    /// </remarks>
    /// <typeparam name="TPage"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="route"></param>
    /// <param name="navigation"></param>
    /// <returns></returns>
    public MauxNavigationBuilder AddContent<TPage, TViewModel>(string route, Action<MauxNavigationBuilder>? navigation = null)
        where TPage : Page
        where TViewModel : class, INotifyPropertyChanged
    {
        _services.TryAddScoped<TViewModel>();

        return AddContent<TPage>(route, navigation);
    }

    /// <summary>
    /// Adds a <see cref="Page"/> of the type specified in <typeparamref name="TPage"/>
    /// to the specified <see cref="IServiceCollection"/> with <see cref="ServiceLifetime.Scoped"/> lifetime
    /// and registers a MAUI Shell route in <see cref="Routing"/> using the value of <typeparamref name="TPage"/>
    /// as the route.
    /// A <see cref="IServiceScope"/> will be created upon route navigation and disposed on element <see cref="Page.Unloaded"/>.
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="navigation"></param>
    /// <returns></returns>
    public MauxNavigationBuilder AddRoute<TPage>(Action<MauxNavigationBuilder>? navigation = null)
        where TPage : Page
        => AddRoute<TPage>(typeof(TPage).Name, navigation);

    /// <summary>
    /// Adds a <see cref="Page"/> of the type specified in <typeparamref name="TPage"/>
    /// to the specified <see cref="IServiceCollection"/> with <see cref="ServiceLifetime.Scoped"/> lifetime
    /// and registers a MAUI Shell route in <see cref="Routing"/> using the value of <paramref name="route"/>
    /// as the route.
    /// A <see cref="IServiceScope"/> will be created upon route navigation and disposed on element <see cref="Page.Unloaded"/>.
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="route"></param>
    /// <param name="navigation"></param>
    /// <returns></returns>
    public MauxNavigationBuilder AddRoute<TPage>(string route, Action<MauxNavigationBuilder>? navigation = null)
        where TPage : Page
    {
        _services.TryAddScoped<TPage>();

        var elementRoute = $"{_baseRoute}/{route}".TrimStart('/');
        _navigationMap.Add(elementRoute, typeof(TPage));
        Routing.RegisterRoute(elementRoute, ScopedRouteFactory<TPage>.Instance);

        navigation?.Invoke(new MauxNavigationBuilder(_services, _navigationMap, elementRoute));

        return this;
    }

    /// <summary>
    /// Adds a <see cref="Page"/> of the type specified in <typeparamref name="TPage"/> and a ViewModel
    /// of the type specified in <typeparamref name="TViewModel"/> to the specified
    /// <see cref="IServiceCollection"/> with <see cref="ServiceLifetime.Scoped"/> lifetime
    /// and registers a MAUI Shell route in <see cref="Routing"/> using the value of <typeparamref name="TPage"/>
    /// as the route.
    /// A <see cref="IServiceScope"/> will be created upon route navigation and disposed on element <see cref="Page.Unloaded"/>.
    /// </summary>
    /// <remarks>
    /// Developers are still responsible for assigning the injected instance of <typeparamref name="TViewModel" /> 
    /// to the BindingContext property of <typeparamref name="TPage" />.
    /// </remarks>
    /// <typeparam name="TPage"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="navigation"></param>
    /// <returns></returns>
    public MauxNavigationBuilder AddRoute<TPage, TViewModel>(Action<MauxNavigationBuilder>? navigation = null)
        where TPage : Page
        where TViewModel : class, INotifyPropertyChanged
        => AddRoute<TPage, TViewModel>(typeof(TPage).Name, navigation);

    /// <summary>
    /// Adds a <see cref="Page"/> of the type specified in <typeparamref name="TPage"/> and a ViewModel
    /// of the type specified in <typeparamref name="TViewModel"/> to the specified
    /// <see cref="IServiceCollection"/> with <see cref="ServiceLifetime.Scoped"/> lifetime
    /// and registers a MAUI Shell route in <see cref="Routing"/> using the value of <paramref name="route"/>
    /// as the route.
    /// A <see cref="IServiceScope"/> will be created upon route navigation and disposed on element <see cref="Page.Unloaded"/>.
    /// </summary>
    /// <remarks>
    /// Developers are still responsible for assigning the injected instance of <typeparamref name="TViewModel" /> 
    /// to the BindingContext property of <typeparamref name="TPage" />.
    /// </remarks>
    /// <typeparam name="TPage"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="route"></param>
    /// <param name="navigation"></param>
    /// <returns></returns>
    public MauxNavigationBuilder AddRoute<TPage, TViewModel>(string route, Action<MauxNavigationBuilder>? navigation = null)
        where TPage : Page
        where TViewModel : class, INotifyPropertyChanged
    {
        _services.TryAddScoped<TViewModel>();

        return AddRoute<TPage>(route, navigation);
    }
}