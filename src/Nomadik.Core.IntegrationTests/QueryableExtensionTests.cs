using System.Linq.Expressions;
using Nomadik.Core.Extensions;
using Nomadik.Extensions;

namespace Nomadik.Core.IntegrationTests;

public class QueryableExtensionTests 
{
    private class From
    {
        internal required int Foo { get; init; }
        internal required int Bar { get; init; }
    }
    
    private class To
    {
        internal required int Phi { get; init; }
        internal required int Baz { get; init; }
    }

    private static IQueryable<From> SetupData()
    {
        return Enumerable.Range(1, 10).Select(i => new From
        {
            Foo = i,
            Bar = i % 2
        }).AsQueryable();
    }

    private static readonly Expression<Func<From, To>> Mapper = f => new()
    {
        Phi = f.Foo,
        Baz = f.Bar
    };

    [Test]
    public void QueryableExtensions_OrderBy_NotNull_Sorts()
    {
        // Arrange
        var data = SetupData();

        var expecteds = data.OrderBy(d => d.Foo).ToList();
        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery()
        {
            Order = new()
            {
                By = nameof(To.Phi),
                Dir = OrderDir.Asc
            }
        };

        var search = ctx.Compile(query);

        // Act
        var searchResult = data.OrderBy(search).ToList(); 

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
    public void QueryableExtensions_OrderBy_Null_Throws()
    {
        // Arrange
        var data = SetupData();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery();

        var search = ctx.Compile(query);

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => 
            data.AsQueryable().OrderBy(search).ToList()
        ); 
    }

    [Test]
    public void QueryableExtensions_TryOrderBy_NotNull_Sorts()
    {
        // Arrange
        var data = SetupData();

        var expecteds = data.OrderByDescending(d => d.Foo).ToList();
        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery()
        {
            Order = new()
            {
                By = nameof(To.Phi),
                Dir = OrderDir.Desc
            }
        };

        var search = ctx.Compile(query);

        // Act
        var searchResult = data.TryOrderBy(search).ToList(); 

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
    public void QueryableExtensions_TryOrderBy_Null_OriginalOrder()
    {
        // Arrange
        var data = SetupData();

        var expecteds = data.ToList();
        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery();

        var search = ctx.Compile(query);

        // Act
        var searchResult = data.TryOrderBy(search).ToList(); 

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
    public void QueryableExtensions_ThenBy_NotNull_Sorts()
    {
        // Arrange
        var data = SetupData();

        var expecteds = data
            .OrderBy(d => d.Bar)
            .ThenBy(d => d.Foo)
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery()
        {
            Order = new()
            {
                By = nameof(To.Phi),
                Dir = OrderDir.Asc
            }
        };

        var search = ctx.Compile(query);

        // Act
        var searchResult = data
            .OrderBy(d => d.Bar)
            .ThenBy(search)
            .ToList(); 

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
    public void QueryableExtensions_ThenBy_Null_Throws()
    {
        // Arrange
        var data = SetupData();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery();

        var search = ctx.Compile(query);

        // Act & Assert
        Assert.Throws<NullReferenceException>(() =>
            data
                .OrderBy(d => d.Bar)
                .ThenBy(search)
                .ToList()
        );
    }

    [Test]
    public void QueryableExtensions_TryThenBy_NotNull_Sorts()
    {
        // Arrange
        var data = SetupData();

        var expecteds = data
            .OrderBy(d => d.Bar)
            .ThenByDescending(d => d.Foo)
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery()
        {
            Order = new()
            {
                By = nameof(To.Phi),
                Dir = OrderDir.Desc
            }
        };

        var search = ctx.Compile(query);

        // Act
        var searchResult = data
            .OrderBy(d => d.Bar)
            .TryThenBy(search)
            .ToList(); 

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
    public void QueryableExtensions_TryThenBy_Null_OriginalOrder()
    {
        // Arrange
        var data = SetupData();

        var expecteds = data
            .OrderBy(d => d.Bar)
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery();

        var search = ctx.Compile(query);

        // Act
        var searchResult = data
            .OrderBy(d => d.Bar)
            .TryThenBy(search)
            .ToList(); 

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
    public void QueryableExtensions_Page_NotNull_Pages()
    {
        // Arrange
        var data = SetupData();

        var expecteds = data
            .Skip(5)
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery()
        {
            Page = new ()
            {
                Num = 2,
                Size = 5
            }
        };

        var search = ctx.Compile(query);

        // Act
        var searchResult = data
            .Page(search)
            .ToList(); 

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
    public void QueryableExtensions_Page_FirstPage_Shortcuts()
    {
        // Arrange
        var data = SetupData();

        var expecteds = data
            .Take(5)
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery()
        {
            Page = new ()
            {
                Num = 1,
                Size = 5
            }
        };

        var search = ctx.Compile(query);

        // Act
        var searchResult = data
            .Page(search)
            .ToList(); 

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
    public void QueryableExtensions_Page_Null_ReturnsOriginal()
    {
        // Arrange
        var data = SetupData();

        var expecteds = data
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery();

        var search = ctx.Compile(query);

        // Act
        var searchResult = data
            .Page(search)
            .ToList(); 

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
    public void QueryableExtensions_Page_Select_Projects()
    {
        // Arrange
        var data = SetupData();

        var expecteds = data
            .Select(Mapper)
            .ToList();

        var ctx = Composition.CreateNomadik(Mapper);
        var query = new SearchQuery();

        var search = ctx.Compile(query);

        // Act
        var searchResult = data
            .Select(search)
            .ToList(); 

        // Assert
        Assert.That(searchResult, Has.Count.EqualTo(expecteds.Count));

        for (var n = 0; n < searchResult.Count; n++)
        {
            var expected = expecteds[n];
            var actual = searchResult[n];

            Assert.Multiple(() =>
            {
                Assert.That(actual.Phi, Is.EqualTo(expected.Phi));
                Assert.That(actual.Baz, Is.EqualTo(expected.Baz));
            });
        }
    }
}
