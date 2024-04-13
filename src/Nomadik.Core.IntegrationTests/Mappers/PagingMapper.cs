using System.Linq.Expressions;
using Nomadik.Core.Abstractions;
using Nomadik.Core.IntegrationTests.Data;
using Nomadik.Core.IntegrationTests.DTOs;

namespace Nomadik.Core.IntegrationTests.Mappers;

public class PagingMapper : IMapperProvider<TestModelPaging, TestPagingDTO>
{
    public Expression<Func<TestModelPaging, TestPagingDTO>> Compile()
    {
        return tmp => new()
        {
            AnInt = tmp.SomeInt,
            AnIntNull = tmp.SomeIntNull,
            AString = tmp.SomeString,
            AStringNull = tmp.SomeStringNull,
            ADecimal = tmp.SomeDecimal,
            ADecimalNull = tmp.SomeDecimalNull,
            ADTO = tmp.SomeDTO,
            ADTONull = tmp.SomeDTONull,
            ADT = tmp.SomeDT,
            ADTNull = tmp.SomeDTNull,
            ABool = tmp.SomeBool,
            ABoolNull = tmp.SomeBoolNull
        };
    }
}
