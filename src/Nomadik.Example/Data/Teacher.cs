namespace Nomadik.Example.Data;

public class Teacher 
{
    public int TeacherId { get; set; }

    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }

    public List<Classroom> Classrooms { get; set; } = [];
}
