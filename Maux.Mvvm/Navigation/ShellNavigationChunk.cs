namespace Maux;

/// <summary>
/// Provides a route name with optional parameters to be passed
/// </summary>
[PublicAPI]
public record ShellNavigationChunk (string RouteName, ShellParameters? ShellParameters)
{
    /// <summary>
    /// Creates a <see cref="ShellNavigationChunk"/> with <see cref="MauxIntent.ParameterName"/> parameter
    /// containing the specified intent
    /// </summary>
    /// <param name="route"></param>
    /// <returns></returns>
    public static implicit operator ShellNavigationChunk((string RouteName, object Intent) route)
        => new(route.RouteName, new ShellParameters { { MauxIntent.ParameterName, route.Intent } });

    /// <summary>
    /// Creates a <see cref="ShellNavigationChunk"/> with specified parameters
    /// </summary>
    /// <param name="route"></param>
    /// <returns></returns>
    public static implicit operator ShellNavigationChunk((string RouteName, ShellParameters Parameters) route)
        => new(route.RouteName, route.Parameters);

    /// <summary>
    /// Creates a <see cref="ShellNavigationChunk"/> without parameters
    /// </summary>
    /// <param name="routeName"></param>
    /// <returns></returns>
    public static implicit operator ShellNavigationChunk(string routeName)
        => new(routeName, null);
}