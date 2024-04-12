using System.Linq.Expressions;
using Nomadik.Core.Abstractions;
using Nomadik.Example.Data;
using Nomadik.Example.DTOs;

namespace Nomadik.Example.MapperProviders;

public class IndexTeachersProvider(ExampleDbContext db) 
    : IMapperProvider<Teacher, IndexTeachersDto>
{
    private ExampleDbContext Db { get; } = db;

    public Expression<Func<Teacher, IndexTeachersDto>> Compile()
    {
        return t => new ()
        {
            Id = t.TeacherId,
            FullName = t.FirstName + " " + t.LastName,
            ClassroomCount = t.Classrooms.Count,
            StudentCount = t.Classrooms.Sum(c => c.Students.Count),
            ClassroomIds = t.Classrooms.Select(c => c.ClassroomId)
        };
    }
}
