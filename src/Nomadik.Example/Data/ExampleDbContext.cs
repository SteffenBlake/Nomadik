using Microsoft.EntityFrameworkCore;

namespace Nomadik.Example.Data;

public class ExampleDbContext(DbContextOptions<ExampleDbContext> options) 
    : DbContext(options)
{
    public required DbSet<Classroom> Classrooms { get; init; }

    public required DbSet<Student> Students { get; init; }

    public required DbSet<Teacher> Teachers { get; init; }
}
