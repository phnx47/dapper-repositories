using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
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
