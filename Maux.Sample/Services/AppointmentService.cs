using Faker;
using Maux.Sample.Models;

namespace Maux.Sample.Services;

public interface IAppointmentService : IModelService<Appointment>
{
    Task InitializeAsync();
}

public class AppointmentService : ModelServiceBase<Appointment>, IAppointmentService
{
    private readonly ICustomerService _customerService;

    public AppointmentService(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task InitializeAsync()
    {
        var customers = await _customerService.QueryAsync();
        var start = DateTimeOffset.Now.AddDays(-15);

        for (int i = 0; i < 1000; i++)
        {
            var customer = customers[RandomNumber.Next(0, customers.Count - 1)];

            start = start.AddMinutes(RandomNumber.Next(0, 120));
            var end = start.AddMinutes(RandomNumber.Next(15, 90));
            
            var appointment = new Appointment(
                Guid.NewGuid().ToString("N"),
                start,
                end,
                customer.Id,
                Lorem.Sentence(5)
            );
            Data.Add(appointment.Id, appointment);

            start = end;
        }
    }
}