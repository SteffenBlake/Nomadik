namespace Nomadik.Example.Data;

public class Classroom 
{
    public int ClassroomId { get; set; }

    public required string RoomNumber { get; set; }

    public int Floor { get; set; }

    public List<Student> Students { get; set; } = [];
    
    public List<Teacher> Teachers { get; set; } = [];
}
