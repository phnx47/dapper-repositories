namespace MicroOrm.Dapper.Repositories.Tests.DatabaseFixture
{
    public class MsSql2008DatabaseFixture : MsSqlDatabaseFixture
    {
        public MsSql2008DatabaseFixture()
            : base("Server=(local)\\SQL2008R2SP2;Database=master;UID=sa;PWD=Password12!")
        {
        }
    }
}
