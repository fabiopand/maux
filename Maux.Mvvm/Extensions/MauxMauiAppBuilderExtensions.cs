using Maux;

// ReSharper disable once CheckNamespace
// ReSharper disable UnusedTypeParameter

namespace Microsoft.Maui.Controls.Hosting;

/// <summary>
/// Maux extensions on top of <see cref="MauiAppBuilder"/>
/// </summary>
[PublicAPI]
public static class MauxMauiAppBuilderExtensions
{
    /// <summary>
    /// Registers your <typeparamref name="TApplication"/> application as <see cref="IMauxApplication"/>. 
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TApplication"></typeparam>
    /// <returns></returns>
    public static MauiAppBuilder UseMauxPageModel<TApplication>(this MauiAppBuilder builder) 
        where TApplication : IMauxApplication
    {
        builder.Services
            .AddSingleton<IMauxApplication>(sp => (IMauxApplication)sp.GetRequiredService<IApplication>());
        
        return builder;
    }

    /// <summary>
    /// Registers <see cref="Shell"/> content/pages routes fluently as <see cref="ServiceLifetime.Scoped"/>.
    /// Provides <see cref="IShellNavigation"/> service to navigate between pages through <see cref="Shell"/>.
    /// </summary>
    /// <remarks>
    /// Each route visual element (and optionally view model) will be added as <see cref="ServiceLifetime.Scoped"/>.
    /// A <see cref="IServiceScope"/> will be created upon navigation and disposed when the page
    /// is <see cref="VisualElement.Unloaded"/> from the shell navigation stack.
    /// </remarks>
    /// <param name="builder"></param>
    /// <param name="navigation"></param>
    /// <returns></returns>
    public static MauiAppBuilder UseShellNavigation(this MauiAppBuilder builder, Action<MauxNavigationBuilder> navigation)
    {
        var navigationMap = new MauxNavigationMap();
        
        builder.Services
            .AddSingleton(navigationMap)
            .AddSingleton<IShellNavigation, ShellNavigation>();

        navigation.Invoke(new MauxNavigationBuilder(builder.Services, navigationMap, string.Empty));

        return builder;
    }
    
    /// <summary>
    /// Provides <see cref="IAppNavigation"/> service to navigate between pages through <see cref="NavigationPage"/>.     
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TApplication"></typeparam>
    /// <returns></returns>
    public static MauiAppBuilder UseAppNavigation<TApplication>(this MauiAppBuilder builder)
        where TApplication : IMauxApplication
    {
        builder.Services
            .AddSingleton<IAppNavigation, AppNavigation>()
            .AddSingleton<IAppNavigationSetter>(sp => (IAppNavigationSetter)sp.GetRequiredService<IAppNavigation>());

        return builder;
    }
}
