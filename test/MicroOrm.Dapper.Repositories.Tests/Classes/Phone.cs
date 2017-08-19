using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;

namespace MicroOrm.Dapper.Repositories.Tests.Classes
{
    [Table("Phones", Schema = "DAB")]
    public class Phone
    {
        [Key]
        [Identity]
        public int Id { get; set; }

        public string Number { get; set; }

        public bool IsActive { get; set; }

        [IgnoreUpdate]
        public string Code { get; set; }
    }
}