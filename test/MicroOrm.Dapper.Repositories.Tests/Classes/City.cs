using System.ComponentModel.DataAnnotations.Schema;


namespace MicroOrm.Dapper.Repositories.Tests.Classes
{
    [Table("Cities")]
    public class City
    {
        public string Identifier { get; set; }

        public string Name { get; set; }

    }
}