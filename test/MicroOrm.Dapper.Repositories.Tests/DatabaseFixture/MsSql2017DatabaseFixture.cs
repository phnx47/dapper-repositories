namespace MicroOrm.Dapper.Repositories.Tests.DatabaseFixture
{
    public class MsSql2017DatabaseFixture : MsSqlDatabaseFixture
    {
        public MsSql2017DatabaseFixture()
            : base("Server=(local)\\SQL2017;Database=master;User ID=sa;Password=Password12!")
        {
        }
    }
}
