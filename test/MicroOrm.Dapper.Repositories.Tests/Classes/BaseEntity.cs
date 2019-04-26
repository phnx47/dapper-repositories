using System.ComponentModel.DataAnnotations;
using MicroOrm.Dapper.Repositories.Attributes;

namespace MicroOrm.Dapper.Repositories.Tests.Classes
{
    public class BaseEntity<TKey>
    {
        [Key, Identity]
        public virtual TKey Id { get; set; }
    }
}
