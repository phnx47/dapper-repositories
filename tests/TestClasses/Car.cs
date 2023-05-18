using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes.LogicalDelete;

namespace TestClasses;

[Table("Cars")]
public class Car : BaseEntity<int>
{
    public string Name { get; set; }

    public byte[] Data { get; set; }

    public int UserId { get; set; }

    [LeftJoin("Users", "UserId", "Id")]
    public User User { get; set; }

    [Status]
    public StatusCar Status { get; set; }
}

public enum StatusCar
{
    Inactive = 0,

    Active = 1,

    [Deleted]
    Deleted = -1
}

[Table(name: "CommandeFournisseur_CoFo", Schema = "S_Cfo")]
public class ProviderOrder
{
    [Key, Identity]
    public int CoFo_ID { get; set; }

    [InnerJoin("S_Cfo.CommandeFournisseurLignes_CfLi", "CoFo_ID", "CoFo_ID", TableAlias = "CFLI")]
    public List<ProviderOrderLine> ProviderOrderLines { get; set; }
}

[Table(name: "CommandeFournisseurLignes_CfLi", Schema = "S_Cfo")]
public class ProviderOrderLine
{
    [Key, Identity]
    public int CfLi_ID { get; set; }

    public int CoFo_ID { get; set; }

    [InnerJoin("S_Cfo.CommandeFournisseurLignesStatut_CfLs", "CfLi_ID", "CfLi_ID", TableAlias = "CFLS")]
    public ProviderOrderStatus ProviderOrderStatus { get; set; }
}

[Table(name: "CommandeFournisseurLignesStatut_CfLs", Schema = "S_Cfo")]
public class ProviderOrderStatus
{
    [Key, Identity]
    public int CfLs_ID { get; set; }

    public int CfLi_ID { get; set; }
}
