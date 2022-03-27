namespace MicroOrm.Dapper.Repositories.Tests.DatabaseFixture
{
    public class MsSql2019DatabaseFixture : MsSqlDatabaseFixture
    {
        public MsSql2019DatabaseFixture()
            : base("Server=localhost;Database=master;User ID=sa;Password=Password12!")
        {
        }
    }
}
