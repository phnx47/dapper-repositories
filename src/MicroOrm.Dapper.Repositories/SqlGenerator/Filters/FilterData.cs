namespace MicroOrm.Dapper.Repositories.SqlGenerator.Filters
{
    /// <summary>
    /// The filter data class; This should have some more things...
    /// </summary>
    public class FilterData
    {
        /// <summary>
        /// The query select settings
        /// </summary>
        public SelectInfo SelectInfo { get; set; }   
        
        /// <summary>
        /// The query order settings
        /// </summary>
        public OrderInfo OrderInfo { get; set; }

        /// <summary>
        /// The query group settings
        /// </summary>
        public GroupInfo GroupInfo { get; set; }

        /// <summary>
        /// The query limits settings
        /// </summary>
        public LimitInfo LimitInfo { get; set; }

        /// <sumary>
        /// Specify if the query is ordered
        /// </sumary>
        public bool Ordered { get; set; }
    }
}
