using CommunityToolkit.Mvvm.ComponentModel;
using Maux.Sample.Services;

namespace Maux.Sample.ViewModels;

public record ShowAppointmentIntent(string AppointmentId);

public partial class AppointmentPageModel : MauxPageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ICustomerService _customerService;

    [ObservableProperty]
    AppointmentViewModel? _appointment;

    public AppointmentPageModel(IAppointmentService appointmentService, ICustomerService customerService)
    {
        _appointmentService = appointmentService;
        _customerService = customerService;
    }
    
    public override Task PrepareAsync(object? intent = null) =>
        intent switch
        {
            ShowAppointmentIntent editAppointment => LoadAppointmentAsync(editAppointment.AppointmentId),
            _ => MauxIntent.ThrowInvalidIntentAsync(intent)
        };

    private async Task LoadAppointmentAsync(string appointmentId)
    {
        var appointment = await _appointmentService.GetAsync(appointmentId);
        var customer = await _customerService.GetAsync(appointment.CustomerId);
        Appointment = new AppointmentViewModel(appointment, customer);
    }
}