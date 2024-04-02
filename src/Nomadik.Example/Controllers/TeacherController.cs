using Microsoft.AspNetCore.Mvc;
using Nomadik.Core;
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
        ExampleDbContext db
    )
    {
        var mapper = IndexTeachersDto.Mapper(db); 
        var compiledQuery = query.Compile(mapper);

        return await db.Teachers.SearchAsync(compiledQuery);
    }
}
