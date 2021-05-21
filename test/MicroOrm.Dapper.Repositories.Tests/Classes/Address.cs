using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;

namespace MicroOrm.Dapper.Repositories.Tests.Classes
{
    [Table("Addresses")]
    public class Address
    {
        [Key]
        [Identity, IgnoreUpdate]
        public int Id { get; set; }

        public string Street { get; set; }

        [LeftJoin("Users", "Id", "AddressId")]
        public List<User> Users { get; set; }

        public string CityId { get; set; }
        
        [InnerJoin("Cities", "CityId", "Identifier")]
        public City City { get; set; }
    }
}
