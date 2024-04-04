using System.Linq.Expressions;
using Nomadik.Core.IntegrationTests.Data;

namespace Nomadik.Core.IntegrationTests.DTOs;

public class DTOC 
{
    public int Id { get; set; }

    public bool HasD { get; set; }

    public static Expression<Func<TestModelC, DTOC>> FromModel(TestDbContext db)
    {
        return c => new()
        {
            Id = c.TestModelCId,
            HasD = db.D.Any(d => d.ArbitraryField == c.ArbitraryField)
        };
    }
}
