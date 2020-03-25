using System.Collections.Generic;

namespace MicroOrm.Dapper.Repositories.SqlGenerator.Filters
{
    /// <summary>
    /// Query Group by info
    /// </summary>
    public class GroupInfo
    {
        /// <summary>
        /// Columns to sort
        /// </summary>
        public List<string> Columns { get; set; }

        /// <summary>
        /// If true, will be used for all queries
        /// </summary>
        public bool Permanent { get; set; }

        /// <summary>
        /// You can specify a custom query if you need more "liberty"
        /// </summary>
        public string CustomQuery { get; set; }
    }
}
