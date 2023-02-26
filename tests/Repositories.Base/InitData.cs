using TestClasses;

namespace Repositories.Base;

public static class InitData
{
    public static void Execute(TestDbContext db)
    {
        db.Address.Insert(new Address { Street = "Street0", CityId = "MSK" });

        db.Phones.Insert(new Phone { PNumber = "123", IsActive = true, Code = "UK" });
        db.Phones.Insert(new Phone { PNumber = "333", IsActive = false, Code = "UK" });

        for (var i = 0; i < 10; i++)
            db.Users.Insert(new User
            {
                Name = $"TestName{i}",
                AddressId = 1,
                PhoneId = 1,
                OfficePhoneId = 2
            });

        db.Users.Insert(new User { Name = "TestName0", PhoneId = 1 });
        db.Cars.Insert(new Car { Name = "TestCar0", UserId = 1 });
        db.Cars.Insert(new Car { Name = "TestCar1", UserId = 1 });

        db.Reports.Insert(new Report() { Id = 1, AnotherId = 2, UserId = 1 });
    }
}
