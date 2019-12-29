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
            TimeZone = TimeZoneInfo.Utc;
        }

        /// <summary>
        /// Specified timezone, default UTC
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; }
    }
}
