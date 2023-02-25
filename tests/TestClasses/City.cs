using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestClasses;

[Table("Cities")]
public class City
{
    public Guid Identifier { get; set; }

    public string Name { get; set; }
}
