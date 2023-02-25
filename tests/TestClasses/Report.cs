using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;

namespace TestClasses;

[Table("Reports")]
public class Report
{
    [Key]
    [IgnoreUpdate]
    public int Id { get; set; }

    [Key]
    public int AnotherId { get; set; }

    public int UserId { get; set; }

    [LeftJoin("Users", "UserId", "Id")]
    public User User { get; set; }
}
