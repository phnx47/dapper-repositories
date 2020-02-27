using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes.LogicalDelete;

namespace MicroOrm.Dapper.Repositories.Tests.Classes
{
    [Table("Cars")]
    public class Car : BaseEntity<int>
    {
        public string Name { get; set; }

        public byte[] Data { get; set; }

        public int UserId { get; set; }

        [LeftJoin("Users", "UserId", "Id")]
        public User User { get; set; }

        [Status]
        public StatusCar Status { get; set; }
    }

    public enum StatusCar
    {
        Inactive = 0,

        Active = 1,

        [Deleted]
        Deleted = -1
    }
}
