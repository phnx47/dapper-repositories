using System;

namespace MicroOrm.Dapper.Repositories.Attributes
{
    /// <inheritdoc />
    /// <summary>
    ///     UpdatedAt. Warning!!! Changes the property during SQL generation
    /// </summary>
    public sealed class UpdatedAtAttribute : Attribute
    {
        /// <summary>
        /// [UpdatedAt] Attribute
        /// </summary>
        public UpdatedAtAttribute()
        {
            TimeKind = DateTimeKind.Utc;
            OffSet = 0;
        }

        /// <summary>
        /// The timezone offset
        /// </summary>
        public int OffSet { get; set; }

        /// <summary>
        /// Specified time kind, default UTC.
        /// </summary>
        public DateTimeKind TimeKind { get; set; }
    }
}
