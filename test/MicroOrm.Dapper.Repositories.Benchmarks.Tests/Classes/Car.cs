using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;

namespace MicroOrm.Dapper.Repositories.Benchmarks.Tests.Classes
{
    [Table("Cars")]
    public class Car
    {
        [Key]
        [Identity]
        public int Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        [LeftJoin("Users", "UserId", "Id")]
        public User User { get; set; }
    }
}