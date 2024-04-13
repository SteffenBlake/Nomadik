using Nomadik.Core.Abstractions;
using Nomadik.Core.Extensions;
using Nomadik.Core.IntegrationTests.Data;
using Nomadik.Core.IntegrationTests.DTOs;
using Nomadik.Extensions;

namespace Nomadik.Core.IntegrationTests;

public class ListTests 
{
    private static async Task<List<TestModelB>> BuildDataAsync(TestDbContext db)
    {
        var models = new int[10].Select((_, i) => new TestModelB()
        {
            TestModelBId = i,
            TestModelAs = [
                new ()
                {
                    TestModelAId = i < 5 ? 10+i : 30+i,
                    TestModelBId = i
                },
                new ()
                {
                    TestModelAId = i < 5 ? 20+i : 40+i,
                    TestModelBId = i
                }
            ]
        }).ToList();

        await db.AddRangeAsync(models);
        await db.SaveChangesAsync();

        return models;
    }

    [Test]
    public async Task List_All_SubFilter_Works()
    {
        // Arrange
        using var db = Composition.CreateDb();
        var nomadik = Composition.CreateNomadik(DTOB.FromModel(db));

        var data = await BuildDataAsync(db);

        var expecteds = data.Where(d => 
            d.TestModelAs.All(a => 
                a.TestModelAId >= 20
            )
        ).ToList();

        var query = new SearchQuery()
        {
            Filter = new SearchFilterWhere()
            {
                Where = new()
                {
                    Key = nameof(DTOB.ModelAIds),
                    Operator = Operator.All,
                    Value = new SearchFilterWhere()
                    {
                        Where = new()
                        {
                            Key = "value",
                            Operator = Operator.GTE,
                            Value = "|20|"
                        }
                    }
                }
            }
        };

        var search = nomadik.Compile(query);

        // Act
        var searchResult = await db.B.SearchAsync(search);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(searchResult.From, Is.EqualTo(1));
            Assert.That(searchResult.To, Is.EqualTo(expecteds.Count));
            Assert.That(searchResult.Of, Is.EqualTo(expecteds.Count));
            Assert.That(searchResult.Results, Has.Count.EqualTo(expecteds.Count));
        });

        for (var n = 0; n < expecteds.Count; n++)
        {
            var expected = expecteds[n];
            var actual = searchResult.Results
                .SingleOrDefault(r => r.Id == expected.TestModelBId);

            Assert.That(actual, Is.Not.Null);
        }
    }
}
