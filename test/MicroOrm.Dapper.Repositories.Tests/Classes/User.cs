using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes.LogicalDelete;

namespace MicroOrm.Dapper.Repositories.Tests.Classes
{
    [Table("Users")]
    public class User : BaseEntity<int>
    {
        public string ReadOnly => "test";

        [Column(Order = 1)]
        public string Name { get; set; }

        public int AddressId { get; set; }

        public int PhoneId { get; set; }

        public int OfficePhoneId { get; set; }

        [LeftJoin("Cars", "Id", "UserId", TableAlias = "Cars_Id")]
        public List<Car> Cars { get; set; }

        [LeftJoin("Addresses", "AddressId", "Id")]
        public Address Addresses { get; set; }

        [InnerJoin("Phones", "PhoneId", "Id", "DAB", TableAlias = "Phones_PhoneId")]
        public Phone Phone { get; set; }

        [InnerJoin("Phones", "OfficePhoneId", "Id", "DAB", TableAlias = "OfficePhone")]
        public Phone OfficePhone { get; set; }

        [Status]
        [Deleted]
        public bool Deleted { get; set; }

        [UpdatedAt]
        public DateTime? UpdatedAt { get; set; }
    }
}
