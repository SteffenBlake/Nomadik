using System.Linq.Expressions;
using Nomadik.Example.Data;

namespace Nomadik.Example.DTOs;

public class IndexTeachersDto 
{
    public required int Id { get; init; }

    public required string FullName { get; init; }

    public required int ClassroomCount { get; init; }

    public required int StudentCount { get; init; }

    public static Expression<Func<Teacher, IndexTeachersDto>> Mapper(
        ExampleDbContext db
    )
    {
        return t => new ()
        {
            Id = t.TeacherId,
            FullName = t.FirstName + " " + t.LastName,
            ClassroomCount = t.Classrooms.Count,
            StudentCount = t.Classrooms.Sum(c => c.Students.Count)
        };
    }
}
