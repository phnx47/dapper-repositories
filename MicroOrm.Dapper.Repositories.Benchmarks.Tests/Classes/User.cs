using MicroOrm.Dapper.Repositories.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroOrm.Dapper.Repositories.Benchmarks.Classes
{
    [Table("Users")]
    public class User
    {
        [Key, Identity]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}