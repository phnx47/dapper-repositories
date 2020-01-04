using System.Collections.Generic;

namespace MicroOrm.Dapper.Repositories.SqlGenerator.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectInfo
    {
        
        /// <summary>
        /// Columns
        /// </summary>
        public Dictionary<string, SqlPropertyMetadata> Columns { get; set; }
    }
}
