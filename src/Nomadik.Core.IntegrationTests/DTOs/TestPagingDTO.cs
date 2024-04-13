namespace Nomadik.Core.IntegrationTests.DTOs;

public class TestPagingDTO 
{
    public required int AnInt { get; set; }

    public int? AnIntNull { get; set; }

    public required string AString { get; set; }

    public string? AStringNull { get; set; }

    public required decimal ADecimal { get; set; }

    public decimal? ADecimalNull { get; set; }

    public required DateTimeOffset ADTO { get; set; }

    public DateTimeOffset? ADTONull { get; set; }

    public required DateTime ADT { get; set; }

    public DateTime? ADTNull { get; set; }

    public required bool ABool { get; set; }

    public bool? ABoolNull { get; set; }
}
