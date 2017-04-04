namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <summary>
    ///     Config for SqlGenerator
    /// </summary>
    public class SqlGeneratorConfig
    {
        /// <summary>
        ///     Type Sql provider
        /// </summary>
        public ESqlConnector SqlConnector { get; set; }

        /// <summary>
        ///     Use quotation marks for TableName and ColumnName
        /// </summary>
        public bool UseQuotationMarks { get; set; }
    }
}