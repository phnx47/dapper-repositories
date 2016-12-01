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
        [Key, Identity]
        public int Id { get; set; }

        public string Street { get; set; }
        public string Number { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        [LeftJoin("Users", "Id", "AddressId")]
        public List<User> Users { get; set; }
    }
}