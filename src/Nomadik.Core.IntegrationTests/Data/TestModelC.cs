using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomadik.Core.IntegrationTests.Data;

public class TestModelC 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int TestModelCId { get; set; }

    public required string ArbitraryField { get; set; }
}
