using CommunityToolkit.Mvvm.ComponentModel;
using Maux.Sample.Models;

namespace Maux.Sample.ViewModels;

public partial class AppointmentViewModel : ObservableObject
{
    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private DateTimeOffset _start;

    [ObservableProperty]
    private DateTimeOffset _end;

    [ObservableProperty]
    private Customer _customer;

    [ObservableProperty]
    private string _title;

    public string StartTime => Start.ToString("t");
    public string EndTime => End.ToString("t");
    public string Time => $"{StartTime} - {EndTime}";
    public string Date => $"{Start:M}";

    public AppointmentViewModel(Appointment appointment, Customer customer)
    {
        _id = appointment.Id;
        _start = appointment.Start;
        _end = appointment.End;
        _title = appointment.Title;
        _customer = customer;
    }
}