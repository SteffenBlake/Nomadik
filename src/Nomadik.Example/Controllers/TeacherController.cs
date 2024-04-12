using Microsoft.AspNetCore.Mvc;
using Nomadik.Core;
using Nomadik.Core.Abstractions;
using Nomadik.Core.Extensions;
using Nomadik.Example.Data;
using Nomadik.Example.DTOs;
using Nomadik.Extensions;
using Nomadik.Extensions.Swagger;

namespace Nomadik.Example.Repositories;

public static class TeacherController 
{
    [NomadikSearch(typeof(IndexTeachersDto))]
    public static async Task<SearchQueryResult<IndexTeachersDto>> Index(
        [FromBody]
        SearchQuery query,
        [FromServices]
        ExampleDbContext db,
        [FromServices]
        INomadik<Teacher, IndexTeachersDto> mapper
    )
    {
        var search = mapper.Compile(query);
        return await db.Teachers.SearchAsync(search);
    }
}
