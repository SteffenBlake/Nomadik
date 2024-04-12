using Nomadik.Core;
using Nomadik.Core.Abstractions;
using Nomadik.Example.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace Nomadik.Example.ExampleResponses;

public class IndexTeachersExample : IExamplesProvider<SearchQuery>
{
    public SearchQuery GetExamples()
    {
        return new()
        {
            Page = new()
            {
                Num = 1,
                Size = 20
            },
            Order = new()
            {
                By = nameof(IndexTeachersDto.FullName),
                Dir = OrderDir.Asc
            },
            Filter = new SearchFilterOr
            {
                Or = [
                    new SearchFilterAnd()
                    {
                        And = [
                            new SearchFilterWhere()
                            {
                                Where = new ()
                               {
                                  Key = nameof(IndexTeachersDto.StudentCount),
                                  Operator = Operator.GT,
                                  Value = 65
                              }
                           },
                           new SearchFilterWhere()
                           {
                              Where = new ()
                              {
                                  Key = nameof(IndexTeachersDto.StudentCount),
                                  Operator = Operator.LT,
                                  Value = 70
                              }
                           }
                       ]
                   },
                   new SearchFilterWhere()
                   {
                       Where = new ()
                       {
                           Key = nameof(IndexTeachersDto.FullName),
                           Operator = Operator.LI,
                           Value = "J%"
                       }
                   }
               ]
            }
        };
    }
}
