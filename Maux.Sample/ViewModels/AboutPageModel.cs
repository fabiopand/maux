using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using Maux.Sample.Pages;
using Maux.Sample.Services;

namespace Maux.Sample.ViewModels;

public partial class AboutPageModel : MauxPageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly IShellNavigation _shellNavigation;

    public AboutPageModel(IAppointmentService appointmentService, IShellNavigation shellNavigation)
    {
        _appointmentService = appointmentService;
        _shellNavigation = shellNavigation;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    async Task NavigateToTomorrowFirstAppointment()
    {
        var date = DateTimeOffset.Now.Date.AddDays(1);
        var appointment = (await _appointmentService.QueryAsync(appointment => appointment.Start.Date == date))
            .FirstOrDefault();
        
        if (appointment is null)
        {
            await Toast.Make("No appointments scheduled for tomorrow").Show();
            return;
        }

        await _shellNavigation.NavigateAsync(
            "//",
            (nameof(AgendaPage), new ChangeDateIntent(date)),
            (nameof(AppointmentPage), new ShowAppointmentIntent(appointment.Id))
        );
    }
}