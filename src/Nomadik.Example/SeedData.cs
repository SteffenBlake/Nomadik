using Nomadik.Example.Data;

namespace Nomadik.Example;

public static class SeedData 
{
    public static async Task Run(ExampleDbContext db, string[] names)
    {
        var classrooms = new bool[100]
            .Select((_, i) => i < 40 ? i+1 : i+61)
            .Select((_, i) => new Classroom()
            {
                Floor = i < 100 ? 1 : 2,
                RoomNumber = i.ToString()
            }).ToArray();
           
        await db.AddRangeAsync(classrooms);
        await db.SaveChangesAsync();

        List<Classroom> GetClassrooms() 
        {
            var n = Random.Shared.Next(classrooms.Length-1);
            return [ classrooms[n], classrooms[n+1] ];
        }

        string GetName() => names[Random.Shared.Next(names.Length)];

        var teachers = new bool[200]
            .Select(_ => new Teacher()
            {
                FirstName = GetName(),
                LastName = GetName(),
                Classrooms = GetClassrooms()
            }).ToArray();

        await db.AddRangeAsync(teachers);

        foreach (var classroom in classrooms)
        {
            var students = new bool[Random.Shared.Next(25, 35)]
                .Select(_ => new Student()
                {
                    FirstName = GetName(),
                    LastName = GetName(),
                    ClassroomId = classroom.ClassroomId,
                    Grade = Random.Shared.Next(1, 13)
                }).ToList();
            await db.AddRangeAsync(students);
        }

        await db.SaveChangesAsync();
    }
}
