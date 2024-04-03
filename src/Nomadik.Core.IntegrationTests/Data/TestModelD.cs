using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomadik.Core.IntegrationTests.Data;

public class TestModelD 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int TestModelDId { get; set; }

    public required string ArbitraryField { get; set; }

    public required string SomeOtherField { get; set; }
}
