namespace Nomadik.Example.Data;

public class Student 
{
    public int StudentId { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required int Grade { get; set; }

    public required int ClassroomId { get; set; }
    public Classroom? Classroom { get; set; }
}
