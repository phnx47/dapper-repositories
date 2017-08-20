using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MicroOrm.Dapper.Repositories.Tests.Classes
{
    [Table("Cities")]
    public class City
    {
        public Guid Identifier { get; set; }

        public string Name { get; set; }

    }
}