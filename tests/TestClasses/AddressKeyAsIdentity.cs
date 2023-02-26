﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;

namespace TestClasses;

[Table("Addresses")]
public class AddressKeyAsIdentity
{
    [Key]
    [IgnoreUpdate]
    public int Id { get; set; }

    public string Street { get; set; }

    [LeftJoin("Users", "Id", "AddressId")]
    public List<User> Users { get; set; }

    public string CityId { get; set; }

    [InnerJoin("Cities", "CityId", "Identifier")]
    public City City { get; set; }
}
