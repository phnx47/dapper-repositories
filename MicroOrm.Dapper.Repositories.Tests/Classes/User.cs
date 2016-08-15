using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes.LogicalDelete;

namespace MicroOrm.Dapper.Repositories.Tests.Classes
{
    [Table("Users")]
    public class User
    {
        [Key, Identity]

        public int Id { get; set; }

        public string ReadOnly => "test";

        public string Name { get; set; }

        [LeftJoin("Cars", "Id", "UserId")]
        public List<Car> Cars { get; set; }

        [Status, Deleted]
        public bool Deleted { get; set; }
    }
}
