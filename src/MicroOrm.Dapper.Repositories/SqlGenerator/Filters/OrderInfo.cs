using System.Collections.Generic;

namespace MicroOrm.Dapper.Repositories.SqlGenerator.Filters
{
    /// <summary>
    ///     Query order info
    /// </summary>
    public class OrderInfo
    {
        /// <summary>
        /// The sorting direction
        /// </summary>
        public SortDirection? Direction { get; set; }

        /// <summary>
        /// Columns to sort
        /// </summary>
        public List<string> Columns { get; set; }

        /// <summary>
        /// You can specify a custom query if you need more "liberty"
        /// </summary>
        public string CustomQuery { get; set; }

        /// <summary>
        /// If true, will be used for all queries
        /// </summary>
        public bool Permanent { get; set; }

        /// <summary>
        ///  Possible sorting Direction
        /// </summary>
        public enum SortDirection
        {
            /// <summary>
            /// Ascending
            /// </summary>
            ASC,

            /// <summary>
            /// Descending
            /// </summary>
            DESC
        }
    }
}
