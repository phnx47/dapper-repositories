using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;

namespace TestClasses;

public class BaseEntity<TKey>
{
    [Key, Identity]
    [Column(Order = 0)]
    public virtual TKey Id { get; set; }
}
