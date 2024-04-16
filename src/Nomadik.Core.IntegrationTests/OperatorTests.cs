using System.Linq.Expressions;
using Nomadik.Core.Abstractions;
using Nomadik.Core.Extensions;
using Nomadik.Extensions;

namespace Nomadik.Core.IntegrationTests;

public class OperatorTests 
{
    private class From
    {
        internal required int Foo { get; init; }
        internal required string Bar { get; init; }
    }
    
    private class To
    {
        internal required int Phi { get; init; }
        internal required string Baz { get; init; }
    }

    private static IQueryable<From> SetupData()
    {
        return Enumerable.Range(0, 9).Select(i => new From
        {
            Foo = i,
            Bar = i.ToString()
        }).AsQueryable();
    }

    private static readonly Expression<Func<From, To>> Mapper = f => new()
    {
        Phi = f.Foo,
        Baz = f.Bar
    };

    [Test]
    public void Operator_String_EQ()
    {
        var data = SetupData();

        var expecteds = data
            .Where(d => d.Bar == "4")
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery()
        {
            Filter = new SearchFilterWhere()
            {
                Where = new()
                {
                    Key = nameof(To.Baz),
                    Operator = Operator.EQ,
                    Value = "4"
                }
            }
        };

        var search = ctx.Compile(query);

        // Act
        var searchResult = data.Where(search).ToList(); 

        // Assert
        
        Assert.That(searchResult, Has.Count.EqualTo(expecteds.Count));

        for (var n = 0; n < searchResult.Count; n++)
        {
            var expected = expecteds[n];
            var actual = searchResult[n];

            Assert.Multiple(() =>
            {
                Assert.That(actual.Foo, Is.EqualTo(expected.Foo));
                Assert.That(actual.Bar, Is.EqualTo(expected.Bar));
            });
        }
    }

    [Test]
    public void Operator_String_NE()
    {
        var data = SetupData();

        var expecteds = data
            .Where(d => d.Bar != "4")
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery()
        {
            Filter = new SearchFilterWhere()
            {
                Where = new()
                {
                    Key = nameof(To.Baz),
                    Operator = Operator.NE,
                    Value = "4"
                }
            }
        };

        var search = ctx.Compile(query);

        // Act
        var searchResult = data.Where(search).ToList(); 

        // Assert
        
        Assert.That(searchResult, Has.Count.EqualTo(expecteds.Count));

        for (var n = 0; n < searchResult.Count; n++)
        {
            var expected = expecteds[n];
            var actual = searchResult[n];

            Assert.Multiple(() =>
            {
                Assert.That(actual.Foo, Is.EqualTo(expected.Foo));
                Assert.That(actual.Bar, Is.EqualTo(expected.Bar));
            });
        }
    }

    [Test]
    public void Operator_String_GT()
    {
        var data = SetupData();

        var expecteds = data
            .Skip(5)
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery()
        {
            Filter = new SearchFilterWhere()
            {
                Where = new()
                {
                    Key = nameof(To.Baz),
                    Operator = Operator.GT,
                    Value = "4"
                }
            }
        };

        var search = ctx.Compile(query);

        // Act
        var searchResult = data.Where(search).ToList(); 

        // Assert
        
        Assert.That(searchResult, Has.Count.EqualTo(expecteds.Count));

        for (var n = 0; n < searchResult.Count; n++)
        {
            var expected = expecteds[n];
            var actual = searchResult[n];

            Assert.Multiple(() =>
            {
                Assert.That(actual.Foo, Is.EqualTo(expected.Foo));
                Assert.That(actual.Bar, Is.EqualTo(expected.Bar));
            });
        }
    }

    [Test]
    public void Operator_String_GTE()
    {
        var data = SetupData();

        var expecteds = data
            .Skip(4)
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery()
        {
            Filter = new SearchFilterWhere()
            {
                Where = new()
                {
                    Key = nameof(To.Baz),
                    Operator = Operator.GTE,
                    Value = "4"
                }
            }
        };

        var search = ctx.Compile(query);

        // Act
        var searchResult = data.Where(search).ToList(); 

        // Assert
        
        Assert.That(searchResult, Has.Count.EqualTo(expecteds.Count));

        for (var n = 0; n < searchResult.Count; n++)
        {
            var expected = expecteds[n];
            var actual = searchResult[n];

            Assert.Multiple(() =>
            {
                Assert.That(actual.Foo, Is.EqualTo(expected.Foo));
                Assert.That(actual.Bar, Is.EqualTo(expected.Bar));
            });
        }
    }

    [Test]
    public void Operator_String_LT()
    {
        var data = SetupData();

        var expecteds = data
            .Take(4)
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery()
        {
            Filter = new SearchFilterWhere()
            {
                Where = new()
                {
                    Key = nameof(To.Baz),
                    Operator = Operator.LT,
                    Value = "4"
                }
            }
        };

        var search = ctx.Compile(query);

        // Act
        var searchResult = data.Where(search).ToList(); 

        // Assert
        
        Assert.That(searchResult, Has.Count.EqualTo(expecteds.Count));

        for (var n = 0; n < searchResult.Count; n++)
        {
            var expected = expecteds[n];
            var actual = searchResult[n];

            Assert.Multiple(() =>
            {
                Assert.That(actual.Foo, Is.EqualTo(expected.Foo));
                Assert.That(actual.Bar, Is.EqualTo(expected.Bar));
            });
        }
    }

    [Test]
    public void Operator_String_LTE()
    {
        var data = SetupData();

        var expecteds = data
            .Take(5)
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery()
        {
            Filter = new SearchFilterWhere()
            {
                Where = new()
                {
                    Key = nameof(To.Baz),
                    Operator = Operator.LTE,
                    Value = "4"
                }
            }
        };

        var search = ctx.Compile(query);

        // Act
        var searchResult = data.Where(search).ToList(); 

        // Assert
        
        Assert.That(searchResult, Has.Count.EqualTo(expecteds.Count));

        for (var n = 0; n < searchResult.Count; n++)
        {
            var expected = expecteds[n];
            var actual = searchResult[n];

            Assert.Multiple(() =>
            {
                Assert.That(actual.Foo, Is.EqualTo(expected.Foo));
                Assert.That(actual.Bar, Is.EqualTo(expected.Bar));
            });
        }
    }
}
