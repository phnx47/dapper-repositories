using System;
using MicroOrm.Dapper.Repositories.Attributes;

namespace TestClasses;

public class UpdatedAtLocal : BaseEntity<int>
{
    public string Name { get; set; }

    [UpdatedAt(TimeKind = DateTimeKind.Local)]
    public DateTime? UpdatedAt { get; set; }
}
