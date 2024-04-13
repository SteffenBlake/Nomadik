using System.ComponentModel.DataAnnotations;

namespace Nomadik.Core.IntegrationTests.Data;

public class TestModelPaging 
{
    [Key]
    public int Id { get; set; }

    public required int SomeInt { get; set; }

    public int? SomeIntNull { get; set; }

    public required string SomeString { get; set; }

    public string? SomeStringNull { get; set; }

    public required decimal SomeDecimal { get; set; }

    public decimal? SomeDecimalNull { get; set; }

    public required DateTimeOffset SomeDTO { get; set; }

    public DateTimeOffset? SomeDTONull { get; set; }

    public required DateTime SomeDT { get; set; }

    public DateTime? SomeDTNull { get; set; }

    public required bool SomeBool { get; set; }

    public bool? SomeBoolNull { get; set; }
}
