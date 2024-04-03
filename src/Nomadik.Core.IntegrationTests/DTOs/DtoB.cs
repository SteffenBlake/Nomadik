using System.Linq.Expressions;
using Nomadik.Core.IntegrationTests.Data;

namespace Nomadik.Core.IntegrationTests.DTOs;

public class DtoB 
{
    public required int Id { get; init; }

    public required IEnumerable<string> ModelAIds { get; init; }

    public static Expression<Func<TestModelB, DtoB>> FromModel(TestDbContext _)
    {
        return b => new()
        {
            Id = b.TestModelBId,
            ModelAIds = b.TestModelAs.Select(a => "|" + a.TestModelAId + "|")
        };
    }
}
