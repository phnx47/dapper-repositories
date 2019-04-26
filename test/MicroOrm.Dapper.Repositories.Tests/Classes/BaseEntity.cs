using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;

namespace MicroOrm.Dapper.Repositories.Tests.Classes
{
    public class BaseEntity<TKey>
    {
        [Key, Identity]
        [Column(Order = 0)]
        public virtual TKey Id { get; set; }
    }
}
