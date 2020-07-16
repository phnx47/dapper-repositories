using System.Collections.Generic;

namespace MicroOrm.Dapper.Repositories.Extensions
{
    internal static class CollectionExtensions
    {
        /// <summary>
        ///     AddRange ICollection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="addCollection"></param>
        public static void AddRange<TInput>(this ICollection<TInput> collection, IEnumerable<TInput> addCollection)
        {
            if (collection == null || addCollection == null)
                return;

            foreach (var item in addCollection)
            {
                collection.Add(item);
            }
        }
    }
}
