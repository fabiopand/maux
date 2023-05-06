using CommunityToolkit.Maui;
using Maux.Sample.Pages;
using Maux.Sample.Services;
using Maux.Sample.ViewModels;
using Microsoft.Extensions.Logging;

namespace Maux.Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIconsRound-Regular.otf", "MaterialIcons");
            })
            .UseMauxPageModel<App>()
            .UseShellNavigation(nav => nav
                .AddContent<AboutPage, AboutPageModel>()
                .AddContent<AgendaPage, AgendaPageModel>(agenda => agenda
                    .AddRoute<AppointmentPage, AppointmentPageModel>()
                )
            )
            ;
#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services
            .AddSingleton<ICustomerService, CustomerService>()
            .AddSingleton<IAppointmentService, AppointmentService>()
            ;

        return builder.Build();
    }
}