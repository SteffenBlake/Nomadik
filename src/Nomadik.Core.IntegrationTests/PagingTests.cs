using Nomadik.Core.Extensions;
using Nomadik.Core.IntegrationTests.Data;
using Nomadik.Core.IntegrationTests.DTOs;
using Nomadik.Core.IntegrationTests.Mappers;
using Nomadik.Extensions;

namespace Nomadik.Core.IntegrationTests;

public class PagingTests 
{
    private static async Task<List<TestModelPaging>> GenerateData(
        TestDbContext db
    )
    {
        var nowDTO = DateTimeOffset.Now;
        var nowDt = DateTime.Now;
        var models = new int[10].Select((_, i) => new TestModelPaging()
        {
            SomeInt = i,
            SomeIntNull = i == 0 ? null : i,
            SomeString = i.ToString(),
            SomeStringNull = i == 0 ? null : i.ToString(),
            SomeDecimal = i + 0.5m,
            SomeDecimalNull = i == 0 ? null : i + 0.5m,
            SomeBool = i >= 5,
            SomeBoolNull = i == 0 ? null : i >= 5,
            SomeDTO = nowDTO.AddDays(i),
            SomeDTONull = i == 0 ? null : nowDTO.AddDays(i),
            SomeDT = nowDt.AddDays(i),
            SomeDTNull = i == 0 ? null : nowDt.AddDays(i)
        }).ToList();

        await db.AddRangeAsync(models);
        await db.SaveChangesAsync();

        return models;
    }

    [TestCase(nameof(TestPagingDTO.AnInt))]
    [TestCase(nameof(TestPagingDTO.AnIntNull))]
    [TestCase(nameof(TestPagingDTO.AString))]
    [TestCase(nameof(TestPagingDTO.AStringNull))]
    [TestCase(nameof(TestPagingDTO.ADecimal))]
    [TestCase(nameof(TestPagingDTO.ADecimalNull))]
    [TestCase(nameof(TestPagingDTO.ADTO))]
    [TestCase(nameof(TestPagingDTO.ADTONull))]
    [TestCase(nameof(TestPagingDTO.ADT))]
    [TestCase(nameof(TestPagingDTO.ADTNull))]
    [TestCase(nameof(TestPagingDTO.ABool))]
    [TestCase(nameof(TestPagingDTO.ABoolNull))]
    public async Task Paging_Desc(string key)
    {
        // Arrange
        using var db = Composition.CreateDb();
        var nomadik = Composition.CreateNomadik(new PagingMapper());

        var data = await GenerateData(db);

        var expecteds = data
            .OrderByDescending(d => d.SomeInt)
            .Skip(5)
            .ToList();

        var query = new SearchQuery()
        {
            Order = new()
            {
                By = key,
                Dir = OrderDir.Desc,
                Then = new()
                {
                    By = nameof(TestPagingDTO.AnInt),
                    Dir = OrderDir.Desc
                }
            },
            Page = new()
            {
                Num = 2,
                Size = 5,
            }
        };

        // Act
        var search = nomadik.Compile(query);
        var searchResult = await db.TestModelPagings
            .SearchAsync(search);

        // Assert
        Assert.That(searchResult.Results, Has.Count.EqualTo(expecteds.Count));

        Assert.Multiple(() =>
        {
            Assert.That(searchResult.Of, Is.EqualTo(data.Count));
            Assert.That(searchResult.From, Is.EqualTo(6));
            Assert.That(searchResult.To, Is.EqualTo(10));
        });

        for (var n = 0; n < expecteds.Count; n++)
        {
            var expected = expecteds[n];
            var actual = searchResult.Results[n];

            try {
                Assert.That(actual.AnInt, Is.EqualTo(expected.SomeInt));
            } 
            catch (Exception)
            {
                var ids = searchResult.Results
                    .Select(r => r.AnInt)
                    .ToList();
                /* Console.WriteLine($"Actual Order: {string.Join(",", ids)}"); */
                throw;
            }
        }
    }
}
