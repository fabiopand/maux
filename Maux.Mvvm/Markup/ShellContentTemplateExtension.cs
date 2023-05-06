namespace Maux;

/// <summary>
/// Gets the data template to create when <see cref="ShellContent"/> becomes active using the provided <see cref="BaseShellItem.Route"/>.
/// </summary>
/// <remarks>
/// Works only with content registered with <see cref="MauxMauiAppBuilderExtensions.UseShellNavigation"/>.
/// </remarks>
public class ShellContentTemplateExtension: IMarkupExtension<DataTemplate>
{
    /// <summary>
    /// Provides the data template to create when <see cref="ShellContent"/> becomes active using the provided <see cref="BaseShellItem.Route"/>.
    /// </summary>
    /// <param name="xamlProvider"></param>
    /// <returns></returns>
    public DataTemplate ProvideValue(IServiceProvider xamlProvider)
    {
        var provideValueTarget = xamlProvider.GetRequiredService<IProvideValueTarget>();
        var shellContent = provideValueTarget.TargetObject as ShellContent;

        ArgumentNullException.ThrowIfNull(shellContent, "ShellContent");

        return new DataTemplate(() =>
        {
            // Go up to the IApplication to get the IServiceProvider
            // At this point the ShellContent is already attached to the element tree
            // We cannot use the xamlProvider here, see: https://github.com/dotnet/maui/issues/8824
            var app = shellContent.Parent!;
            while (app is not Shell) app = app.Parent!;
            var serviceProvider = app.Handler!.MauiContext!.Services;
            
            var scope = serviceProvider.CreateScope();
            var navigationMap = scope.ServiceProvider.GetRequiredService<MauxNavigationMap>();
            var contentType = navigationMap.GetRouteType(shellContent.Route);
            var element = (VisualElement)scope.ServiceProvider.GetRequiredService(contentType);
            element.Unloaded += (_, _) => scope.Dispose();

            return element;
        });
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}
