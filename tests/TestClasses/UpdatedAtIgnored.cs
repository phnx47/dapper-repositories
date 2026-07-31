using System;
using MicroOrm.Dapper.Repositories.Attributes;

namespace TestClasses;

public class UpdatedAtIgnored : BaseEntity<int>
{
    public string Name { get; set; }

    [UpdatedAt]
    [IgnoreUpdate]
    public DateTime? UpdatedAt { get; set; }
}
