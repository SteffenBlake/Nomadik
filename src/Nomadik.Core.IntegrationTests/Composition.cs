using Microsoft.EntityFrameworkCore;
using Nomadik.Core.IntegrationTests.Data;

namespace Nomadik.Core.IntegrationTests;

internal class Composition 
{
    internal static TestDbContext CreateDb() 
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(
                Guid.NewGuid().ToString(),
                options => options.EnableNullChecks()
            );

        return new (options.Options);
    }
}
