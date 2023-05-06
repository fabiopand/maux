using CommunityToolkit.Mvvm.ComponentModel;
using Maux.Sample.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Maux.Sample.Pages;

namespace Maux.Sample.ViewModels;

public record ChangeDateIntent(DateTimeOffset Date);

public partial class AgendaPageModel : MauxPageModel
{
    private readonly IShellNavigation _shellNavigation;
    private readonly IAppointmentService _appointmentService;
    private readonly ICustomerService _customerService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    private DateTimeOffset _currentDate;

    [ObservableProperty]
    private ObservableCollection<AppointmentViewModel> _appointments = new();

    public string Title => $"{CurrentDate:M} Agenda";

    public AgendaPageModel(
        IShellNavigation shellNavigation,
        IAppointmentService appointmentService,
        ICustomerService customerService)
    {
        _shellNavigation = shellNavigation;
        _appointmentService = appointmentService;
        _customerService = customerService;

        var now = DateTimeOffset.Now;
        CurrentDate = new DateTimeOffset(now.Date);
    }

    public override Task PrepareAsync(object? intent = null)
    {
        if (intent is ChangeDateIntent changeDateIntent)
        {
            CurrentDate = changeDateIntent.Date;
        }
        return LoadAppointmentsAsync();
    }

    private async Task LoadAppointmentsAsync()
    {
        var date = CurrentDate;
        var dayAfter = CurrentDate.AddDays(1);
        var appointments = await _appointmentService.QueryAsync(appointment => appointment.Start >= date && appointment.Start < dayAfter);
        var customerIds = appointments.Select(a => a.CustomerId).ToHashSet();
        var customers = (await _customerService.QueryAsync(c => customerIds.Contains(c.Id))).ToDictionary(c => c.Id);

        Appointments = new ObservableCollection<AppointmentViewModel>(appointments
                            .OrderBy(appointment => appointment.Start)
                            .Select(appointment => new AppointmentViewModel(appointment, customers[appointment.CustomerId])));
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenAppointment(AppointmentViewModel appointmentViewModel)
    {
        await _shellNavigation.NavigateAsync<AppointmentPage>(new ShowAppointmentIntent(appointmentViewModel.Id));
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private Task PrevDay()
    {
        CurrentDate = CurrentDate.AddDays(-1);
        return LoadAppointmentsAsync();
    }
    
    [RelayCommand(AllowConcurrentExecutions = false)]
    private Task NextDay()
    {
        CurrentDate = CurrentDate.AddDays(1);
        return LoadAppointmentsAsync();
    }
}