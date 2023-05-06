using Maux.Sample.Models;

namespace Maux.Sample.Services;

public interface ICustomerService : IModelService<Customer> { }

public class CustomerService : ModelServiceBase<Customer>, ICustomerService
{
    public CustomerService()
    {
        for (var i = 0; i < 100; i++)
        {
            var fullName = Faker.Name.FullName();
            var id = Guid.NewGuid().ToString("N");
            var email = Faker.Internet.Email(fullName);
            var avatar = $"https://avatars.githubusercontent.com/u/{Faker.RandomNumber.Next(1, 100000000)}";
            Data.Add(id, new Customer(id, fullName, avatar, email));
        }
    }
}