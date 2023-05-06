namespace Maux.Sample.Models;

public record Customer(
    string Id,
    string FullName,
    string AvatarUrl,
    string? Email
) : ModelBase(Id);