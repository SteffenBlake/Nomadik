# Nomadik

Nomadik is a set of dotnet libraries aimed at solving a very common problem that is encountered in the problem space of Object Relational Mappers and standard CRUD APIs / Applications.

The problem space is as follows:

1. We have defined a given mechanism by which to map a Database Entity `E` to an ViewModel `M` (Typically in the form of an `Expression<Func<E, M>>`)
```
public static Expression<Func<StudentModel, StudentDTO>> StudentToStudentDTO= m => new()
{
    FullName = m.FirstName + " " + m.LastName,
    ClassroomId = m.Classroom.ClassroomId,
    Teachers = m.Classroom.Teachers.Select(t =>
        t.Honoric + " " + t.LastName
    ).ToList()
}
```

2. Our API needs the ability to have a form of Queryable, Searchable, Filterable, Sortable, Pageable endpoint for the above mapping

| Student Name  ↕| Classroom Id ↕| Teachers                 ↕|
|----------------|---------------|---------------------------|
| Steven Black   | 102           | Mr. Tallow, Mrs. Jeffries |
| Jace Allen     | 212           | Ms. Kalley                |
| Alexis Smith   | 115           | Mrs. Jackson, Mr Brekton  |

`Page 2 of 5`


3. We want the ability to perform the above "Search" operations *with respect to the properties on the ViewModel `M`
4. *However*, most ORMs support functionality to run those very "Search" operations on the database itself (Typically in the form of an `Expression<Func<E, bool>>`)

```
var filteredStudents = db.Students
    .Where(s => s.FirstName == "Steven");
```

5. *But* the fields for `M` may be complex derived properties that come from operations performed on fields from `E`

```
db.Students
    .OrderByDescending(s => s.FullName);

// The above won't compile, as Student doesnt have a .FullName, it has .FirstName and .LastName!
```

**The common solution often results in having to effectively define all these mappings in two directions**, once for mapping `E` -> `M` for the "read" operation, and a *second* time for mapping `M` properties and "translating" them into boolean "search" operations.

```
if (query.OrderBy.Key == nameof(StudentToStudentDTO.FullName)
{
    if (query.OrderBy.Dir == Dir.Asc)
    {
        students = students.OrderBy(s => s.FirstName + " " + s.LastName);
    }
    else
    {
        students = students.OrderByDescending(s => s.FirstName + " " + s.LastName);
    }
}
```

Which of course is a huge pain and adds a *lot* of extra boilerplate code, and furthermore introduces the pain point of having two sources of truth for the same operations.

If you modify how you map `E.SomeField` to `M.SomeOtherField`, you *also* need to remember to update the matching logic over in serializing your `SearchForM.SomeOtherField` DTO field into that `Expression<Func<M, bool>>`.

```
QA Tester: Bug Report! Sorting order on Students seems to be completely broken after yesterday's changes, what happened?

Dev: Oh shoot, I think I know what happpned, we switched the display mapping to now be "LastName, FirstName" instead of "FirstName LastName", but,
     we forgot to update the corrosponding search and filter operations to do the same...
```

So, the question is as such:

1. Given:
  - An existing defined `Expression<Func<E, M>>` that performs complex operations to derive "compound" or "complex" fields from E -> M
  - A "Search" DTO that searches on the above given properties of `M`
2. Can we deconstruct the `Expression<Func<E, M>>` mapping expression tree, and reconstruct it into a `Expression<Func<E, bool>>` via the "Search" DTO

**The answer, it turns out, is Yes!**

# API Documentation
https://steffenblake.github.io/Nomadik/api/Nomadik.Core.html

# What does it look like?

Heres an example chunk of code below that will demonstrate the library in action:

Given the below:
```
public class Teacher 
{
    public int TeacherId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}

public class TeacherDTO
{
    public required int Id { get; init; }
    public required string FullName { get; init; }

    public static Expression<Func<Teacher, TeacherDTO>> FromModel = t => new ()
        {
            Id = t.TeacherId,
            FullName = t.FirstName + " " + t.LastName
        };
}
```

We start by registering the mapping on our DependencyInjection system:

```
builder.Services.AddNomadik(options =>
{
    options.AddMapper(TeacherDTO.FromModel);
});
```

Assuming we have an existing `IQueryable<Teacher> teachers` from our preferred ORM, and a `SearchQuery` passed in from some form of source (serializable POCO, you can accept it as a param on your API Endpoint(s) for example), we can now inject the built up `INomadik<TIn, TOut>` interface to compile the search query into a LINQ compatible interface:

```
pubic class TeachersController(MyDbContext db, INomadik<Teacher, TeacherDTO> teacherMapper)
{
    private readonly MyDbContext _db = db;
    private readonly INomadik<Teacher, TeacherDto> _teacherMapper = teacherMapper;

    public async Task<SearchQueryResult<TeacherDTO>> SearchTeachers(SearchQuery query)
    {
        var teacherSearch = query.Compile(_teacherMapper);
        return _db.Teachers.SearchAsync(teacherSearch);
    }
}
```

# What does a SearchQuery Support?

The search query API object is comprised of three parts, each of which is nullable and wont apply if set to null by the caller. The entire object as a whole has been designed to be fully serializable in any language (json, yaml, xml, etc)

`Filter`: This is an infinitely nestable `SearchFilter` object, which can be one of several types discriminated by its single field it posses. The supported operations are `where`, `and`, `or`, and `not`.

`Order`: This is an infinitely nestable `SearchOrder` object, which represents a series of "Order By, then by, then by...` set of operations.

`Page`: is a single layer deep object that represents a request page, by a given page size.

## Search Filters

### And

Schema:
```
{
    "and": [ ... SearchFilter ... ]
}
```

The And Filter supports from one to many "child" search filter objects in an array, and will perform an "And" operation union of each of their outputs.

### Or

Schema:
```
{
    "or": [ ... SearchFilter ... ]
}
```

The Or Filter supports from one to many "child" search filter objects in an array, and will perform an "Or" operation union of each of their outputs.

### Not

Schema:
```
{
    "not": SearchFilter
}
```

The Not Filter takes only a single SearchFilter child, and inverts its output True<->False.

### Where

Schema:
```
{
    "where": SearchFilterOperation
}
```

Where filters represent the "leaves" of the tree, defining the actual operations to filter on. For all intents and purposes all branches of whatever a search filter is composed of should terminate with a final "where" leaf at the bottom per branch, each containing a Search Filter Operation as a child

### Search Filter Operation

Schema:
```
{
    "key": string,
    "operator": operator enum, string,
    "value": anything
}
```

The meat of the filter, Search Operations effectively are a comparative between a Key on the DTO, and a value, through an Operator.

### Operators

The complete list of supported operators is here: https://steffenblake.github.io/Nomadik/api/Nomadik.Core.Abstractions.Operator.html

## Search Orders
Schema:
```
{
    "by": string,
    "dir": OrderDir enum, string,
    "then": SearchOrder, nullable
}
```

Performs an "OrderBy" operation, `by` must corrospond to a supported DTO field, and `dir` will default to `Asc` if not specified. If a `then` field is supplied it will chain another `Then By` operation after, and this can be chained as many times as you wish via further nesting of `then` fields.

## Search Pagination
Schema:
```
{
    "num": int,
    "size": int
}
```

Further filters down the search to a paginated result. `num` specifies which page to use and is 1 based, and `size` describes what page size to use per page.

## Example Json Search Query

For the above `TeacherDTO` that was described, the following would be a valid SearchQuery in Json format:
```
{
    // Fetch results 11-20 (page 2, 10 per page)
    "page": {
        "num": 2,
        "size": 10
    },
    // Order by FullName, then by Id
    "order": {
       "by": "FullName",
       "dir": "Asc",
       "then": {
         "by": "Id",
         "dir": "Asc"
       }
    },
    // Get all teachers who's name starts with P, OR, Id is between the range of 0 to 100 (Greater than or equal to 0 AND less than or equal to 100)
    "filter": {
        "or": [
            "where": {
                "key": "FullName",
                "operator": "like",
                "value": "P%"
            },
            "and": [
                "where": {
                    "key": "Id",
                    "operator": "GTE",
                    "value": "0"
                },
                "where": {
                    "key": "Id",
                    "operator": "LTE",
                    "value": "100"
                }
            ]
        ]
    }
}
```

# How to install it

```
dotnet add package Nomadik.Core
```

# Dependency Injection Support

It is also recommended to install the dependency injection package to improve caching of query resources and improve response times of requests. Without using this feature, nomadik queries will have to be composed from scratch every single time.

```
dotnet add package Nomadik.Extensions.DependencyInjection
```

You can then add Nomadik in your `program.cs` via the following:
```
builder.Services.AddNomadik(options =>
{
    ...
});
```

## Registering a mapping

There are two ways to register a mapper for DI, static vs dynamic.

### Static Mapping registration
This is the easiest and most performant, use this approach whenever possible for best performance, as the entire mapping will be pre-cached and singleton.

All it requires is defining an `Expression<Func<FromModel, ToDTO>>` expression in your application.
```
public class Teacher 
{
    public int TeacherId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}

public class TeacherDTO
{
    public required int Id { get; init; }
    public required string FullName { get; init; }

    public static Expression<Func<Teacher, TeacherDTO>> FromModel = t => new ()
        {
            Id = t.TeacherId,
            FullName = t.FirstName + " " + t.LastName
        };
}
```
You can now register that expression like so:
```
builder.Services.AddNomadik(options =>
{
    options.AddMapper(TeacherDTO.FromModel);
});
```

### Dynamic Mapping
For cases where your expression needs to access dynamically available resources, and thus needs to be composed on the fly (for example if you need to access other dependency injected resources mid-expression), then you can instead leverage the [IMapperProvider<TIn, TOut>](https://steffenblake.github.io/Nomadik/api/Nomadik.Core.Abstractions.IMapperProvider-2.html) interface.

```
public class TeacherDtoMapperProvider 
(
    MyAppConfig config // Dependency Injection services can be accessed!
): IMapperProvider<Teacher, TeacherDTO>
{
    public Expression<Func<Teacher, TeacherDTO>> Compile()
    {
        if (config.IncludeTeachersFirstName)
        {
            return t => new ()
            {
                Id = t.TeacherId,
                FullName = t.FirstName + " " + t.LastName
            };
        }
        else
        {
            return t => new ()
            {
                Id = t.TeacherId,
                FullName = t.Honorific + " " + t.LastName
            };
        }
    }
}
```
Then you can register this via one of two methods, depending on whether you need to register it as a Singleton or a Transient. [.AddProviderScoped(...)](https://steffenblake.github.io/Nomadik/api/Nomadik.Extensions.DependencyInjection.NomadikOptions.html#Nomadik_Extensions_DependencyInjection_NomadikOptions_AddProviderScoped__3) vs [.AddProviderSingleton(...)](https://steffenblake.github.io/Nomadik/api/Nomadik.Extensions.DependencyInjection.NomadikOptions.html#Nomadik_Extensions_DependencyInjection_NomadikOptions_AddProviderSingleton__3)

See the [official Microsoft documentation](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes) for distinctions on which you want to use for your Mapper Provider.

```
builder.Services.AddNomadik(options =>
{
    options.AddProviderScoped<MyScopedTeacherMapperProvider, Teacher, TeacherDTO>();
    options.AddProviderSingleton<MySingletonStudentMapperProvider, Student, StudentDTO>();
});
```

## Consuming the registered mapping

Once the registration is done, you can freely inject an `INomadik<TIn, TOut>` matching the same `TIn` + `TOut` of whichever of the above options you chose.

The `INomadik` interface then must be combined with a `SearchQuery` to produce a [CompiledSearchQuery<TIn,Tout>](https://steffenblake.github.io/Nomadik/api/Nomadik.Core.CompiledSearchQuery-2.html), which is now ready to be used for operations on `IQueryable<TIn>`

```
pubic class TeachersController(MyDbContext db, INomadik<Teacher, TeacherDTO> teacherMapper)
{
    private readonly MyDbContext _db = db;
    private readonly INomadik<Teacher, TeacherDto> _teacherMapper = teacherMapper;

    public async Task<SearchQueryResult<TeacherDTO>> SearchTeachers(SearchQuery query)
    {
        var teacherSearch = query.Compile(_teacherMapper);
        ...
    }
}
```

## Supported Operations

The complete set of operations you can perform with a search query are listed here in detail: https://steffenblake.github.io/Nomadik/api/Nomadik.Extensions.IQueryableExtensions.html

# Swagger Support

Due to the complicated nature of the `SearchQuery` object, at this time Dotnet's Swagger OpenAPI generation struggles with serializing it to an OpenAPI spec, even though it is technically viable to serialize.

To this end I have added in a secondary nuget package `Nomadike.Extensions.Swagger` which you can install via:
```
dotnet add package Nomadik.Extensions.Swagger
```

You can then enable Swagger support via adding the following to your `Program.cs`, which will register the appropriate Document Filters for Nomadik to parse SearchQueries:
```
builder.Services.AddSwaggerGen(options =>
{
    ...

    options.AddNomadik();

    ...
});
```

Then, to register a given endpoint to have its documentation generated properly:
```
[NomadikSearch(typeof(YourDto))]
public async Task<SearchQueryResult<YourDto>> Index(
    [FromBody]
    SearchQuery query,
)
{
    ...
}
```
Where `YourDto` represents the *output* object that will be returned to the front end, with it's fields that are Searchable/Filterable/Pageable/etc.

By default, all **Public exposed Properties with a Getter* will be serialized as valid fields to search/filter/sort on. This will then generate a suite of bespoke `YourDTOSearch...` Schemas on the OpenAPi schema **that have a special enum defined for each of the fields**

**Currently at this time, due to the recursive and complex nature of the SearchQuery object, OpenAPI does not play nice with supporting it as a param in anywhere other than the Body**

To that end, you are tightly coupled to largely only supporting SearchQuery endpoints as `POST`.

If you don't care about OpenAPI support though, then it should work fine (and you can probably ignore this entire section of the documentation, why are you even reading this far anyways?)
