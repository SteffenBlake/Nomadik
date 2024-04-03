using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomadik.Core.IntegrationTests.Data;

public class TestModelB 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int TestModelBId { get; set; } 

    public List<TestModelA> TestModelAs { get; set; } = [];
}
