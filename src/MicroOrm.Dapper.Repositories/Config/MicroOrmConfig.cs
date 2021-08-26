using MicroOrm.Dapper.Repositories.SqlGenerator;

namespace MicroOrm.Dapper.Repositories.Config
{
    /// <summary>
    /// This class is used to support dependency injection
    /// </summary>
    public static class MicroOrmConfig
    {
        /// <summary>
        ///     Type Sql provider
        /// </summary>
        public static SqlProvider SqlProvider { get; set; }

        /// <summary>
        ///     Use quotation marks for TableName and ColumnName
        /// </summary>
        public static bool UseQuotationMarks { get; set; }

        /// <summary>
        ///     Prefix for tables
        /// </summary>
        public static string TablePrefix { get; set; }
        
        /// <summary>
        ///     Allow Key attribute as Identity if Identity is not set
        /// </summary>
        public static bool AllowKeyAsIdentity { get; set; }
    }
}
