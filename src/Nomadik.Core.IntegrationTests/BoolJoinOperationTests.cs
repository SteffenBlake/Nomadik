using Nomadik.Core.Abstractions;
using Nomadik.Core.IntegrationTests.Data;
using Nomadik.Core.IntegrationTests.DTOs;
using Nomadik.Extensions;

namespace Nomadik.Core.IntegrationTests;

public class BoolJoinOperationTests 
{
    private static async Task BuildDataAsync(TestDbContext db)
    {
        for (var c = 0; c < 10; c++)
        {
            db.C.Add(new()
            {
                TestModelCId = c,
                ArbitraryField = c.ToString(),
            });
        }

        for (var d = 0; d < 5; d++)
        {
            db.D.Add(new()
            {
                TestModelDId = d,
                ArbitraryField = d.ToString(),
                SomeOtherField = (d*10).ToString()
            });
        }
        await db.SaveChangesAsync();
    }

    [Test]
    public async Task BoolJoinOperation_FilterOn_Works()
    {
        // Arrange
        using var db = Composition.CreateDb();
        await BuildDataAsync(db);

        var mapping = DTOC.FromModel(db);

        var query = new SearchQuery()
        {
            Filter = new SearchFilterWhere()
            {
                Where = new ()
                {
                    Key = nameof(DTOC.HasD),
                    Operator = Operator.EQ,
                    Value = true
                }
            }
        };
        var search = Nomadik.Compile(query, mapping);

        // Act
        var result = await db.C.SearchAsync(search);

        // Assert
        Assert.Multiple(() => 
        {
            Assert.That(result.From, Is.EqualTo(1));
            Assert.That(result.To, Is.EqualTo(5));
            Assert.That(result.Of, Is.EqualTo(5));
            Assert.That(result.Results, Has.Count.EqualTo(5));
        });

        for (var n = 0; n < result.Results.Count; n++)
        {
            var resultC = result.Results[n];
            Assert.Multiple(() =>
            {
                Assert.That(resultC.Id, Is.EqualTo(n));
                Assert.That(resultC.HasD, Is.True);
            });
        }
    }

    [Test]
    public async Task BoolJoinOperation_OrderOn_Works()
    {
        // Arrange
        using var db = Composition.CreateDb();
        await BuildDataAsync(db);

        var mapping = DTOC.FromModel(db);

        var query = new SearchQuery()
        {
            Order = new ()
            {
                By = nameof(DTOC.HasD),
                Dir = OrderDir.Asc
            }
        };
        var search = Nomadik.Compile(query, mapping);

        // Act
        var result = await db.C.SearchAsync(search);

        // Assert
        Assert.Multiple(() => 
        {
            Assert.That(result.From, Is.EqualTo(1));
            Assert.That(result.To, Is.EqualTo(10));
            Assert.That(result.Of, Is.EqualTo(10));
            Assert.That(result.Results, Has.Count.EqualTo(10));
        });

        for (var n = 0; n < 5; n++)
        {
            var resultC = result.Results[n];
            Assert.Multiple(() =>
            {
                Assert.That(resultC.HasD, Is.False);
            });
        }
        for (var n = 5; n < 10; n++)
        {
            var resultC = result.Results[n];
            Assert.Multiple(() =>
            {
                Assert.That(resultC.HasD, Is.True);
            });
        }
    }
}
