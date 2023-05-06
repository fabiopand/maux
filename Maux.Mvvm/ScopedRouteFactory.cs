namespace Maux;

/// <summary>
/// A route factory which creates the element in a scoped container
/// </summary>
/// <remarks>
/// Scope is disposed upon element <see cref="VisualElement.Unloaded"/>.
/// </remarks>
/// <typeparam name="T"></typeparam>
public class ScopedRouteFactory<T> : RouteFactory
    where T: Page
{
    /// <summary>
    /// Singleton route factory instance
    /// </summary>
    public static ScopedRouteFactory<T> Instance { get; } = new();
    
    /// <inheritdoc cref="RouteFactory.GetOrCreate()"/>
    public override Element GetOrCreate()
    {
        var services = Application.Current!.Handler!.MauiContext!.Services;
        return GetOrCreate(services);
    }

    /// <inheritdoc cref="RouteFactory.GetOrCreate(IServiceProvider)"/>
    public override Element GetOrCreate(IServiceProvider services)
    {
        var scope = services.CreateScope();
        
        var element = scope.ServiceProvider.GetRequiredService<T>();
        element.Unloaded += (_, _) => scope.Dispose();

        return element;
    }
}