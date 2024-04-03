using Microsoft.EntityFrameworkCore;

namespace Nomadik.Core.IntegrationTests.Data;

public class TestDbContext(DbContextOptions<TestDbContext> options) :
    DbContext(options)
{
   public DbSet<TestModelA> A { get; set; }

   public DbSet<TestModelB> B { get; set; } 

   public DbSet<TestModelC> C { get; set; } 

   public DbSet<TestModelD> D { get; set; } 
}
