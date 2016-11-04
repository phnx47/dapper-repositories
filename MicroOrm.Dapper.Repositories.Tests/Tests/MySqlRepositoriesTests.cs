using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xunit;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;


namespace MicroOrm.Dapper.Repositories.Tests.Tests
{
    public class MySqlRepositoriesTests : IClassFixture<MySqlDatabaseFixture>
    {
        private readonly MySqlDatabaseFixture _sqlDatabaseFixture;

        public MySqlRepositoriesTests(MySqlDatabaseFixture msSqlDatabaseFixture)
        {
            _sqlDatabaseFixture = msSqlDatabaseFixture;
        }


        //[Fact]
        //public void Insert()
        //{
        //    var t = _sqlDatabaseFixture.Db.Cars.Find();
        //    var image = ByteArrayToImage(t.Data);
        //    image.Save("test.jpg");

        //    var car = new Car
        //    {
        //        Name = "test",
        //        Status = StatusCar.Active,
        //        UserId = 1
        //    };


        //    Image image = Image.FromFile("image.jpg");
        //    car.Image = ImageToByteArray(image);

        //    _sqlDatabaseFixture.Db.Cars.Insert(car);
        //}


       
    }
}