using Maux.Sample.Services;

namespace Maux.Sample.Pages;

public partial class InitializationPage
{
    private readonly IServiceProvider _serviceProvider;

    public InitializationPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Initialized required services
        await _serviceProvider.GetRequiredService<IAppointmentService>().InitializeAsync();

        Application.Current!.MainPage = new AppShell();
    }
}