namespace Maux.Sample.Models;

public record Appointment(
    string Id,
    DateTimeOffset Start,
    DateTimeOffset End, 
    string CustomerId,
    string Title
) : ModelBase(Id);