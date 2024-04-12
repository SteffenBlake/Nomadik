namespace Nomadik.Example.DTOs;

public class IndexTeachersDto 
{
    public required int Id { get; init; }

    public required string FullName { get; init; }

    public required int ClassroomCount { get; init; }

    public required int StudentCount { get; init; }

    public required IEnumerable<int> ClassroomIds { get; init; }
}
