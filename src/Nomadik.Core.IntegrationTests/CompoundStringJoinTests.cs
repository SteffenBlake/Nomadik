using Nomadik.Core.IntegrationTests.Data;
using Nomadik.Core.IntegrationTests.DTOs;
using Nomadik.Extensions;

namespace Nomadik.Core.IntegrationTests;

public class CompoundStringJoinTests
{
    private static async Task BuildDataAsync(TestDbContext db)
    {
        for (var b = 0; b <= 9; b++)
        {
            db.B.Add(new()
            {
                TestModelBId = b
            });
            for (var a = 0; a <= 9; a++)
            {
                db.A.Add(new()
                {
                    TestModelAId = b*10 + a,
                    TestModelBId = b
                });
            }
        }
        await db.SaveChangesAsync();
    }

    [Test]
    public async Task CompoundListSelect_Where_Works()
    {
        // Arrange
        using var db = Composition.CreateDb();
        await BuildDataAsync(db);

        var mapping = DtoB.FromModel(db);

        var query = new SearchQuery()
        {
            Filter = new SearchFilterWhere()
            {
                Where = new()
                {
                    Key = nameof(DtoB.ModelAIds),
                    Operator = Operator.CO,
                    Value = "|1|"
                }
            }
        };
        var compiledQuery = query.Compile(mapping);
        
        // Act
        var result = await db.B.SearchAsync(compiledQuery);
        
        // Assert
        Assert.Multiple(() => 
        {
            Assert.That(result.From, Is.EqualTo(1));
            Assert.That(result.To, Is.EqualTo(1));
            Assert.That(result.Of, Is.EqualTo(1));
            Assert.That(result.Results, Has.Count.EqualTo(1));
            Assert.That(result.Results[0].Id, Is.EqualTo(0));
        });
    }
}
