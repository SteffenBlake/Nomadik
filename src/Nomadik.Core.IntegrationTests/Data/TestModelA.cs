using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomadik.Core.IntegrationTests.Data;

public class TestModelA 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int TestModelAId { get; set; }

    public required int TestModelBId { get; set; }
    public TestModelB? TestModelB { get; set; }
}
