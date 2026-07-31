using System;
using MicroOrm.Dapper.Repositories.Attributes;

namespace TestClasses;

public class UpdatedAtOffSet : BaseEntity<int>
{
    public string Name { get; set; }

    [UpdatedAt(OffSet = 3)]
    public DateTime? UpdatedAt { get; set; }
}
