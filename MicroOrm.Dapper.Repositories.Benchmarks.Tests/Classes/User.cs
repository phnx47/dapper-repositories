using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;

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
