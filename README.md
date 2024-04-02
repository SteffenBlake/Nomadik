# Nomadik

Nomadik is a set of dotnet libraries aimed at solving a very common problem that is encountered in the problem space of Object Relational Mappers and standard CRUD APIs / Applications.

The problem space is as follows:

1. We have defined a given mechanism by which to map a Database Entity `E` to an ViewModel `M` (Typically in the form of an `Expression<Func<E, M>>`)
2. Our API needs the ability to have a form of Queryable, Searchable, Filterable, Sortable, Pageable endpoint for the above mapping
3. We want the ability to perform the above "Search" operations *with respect to the properties on the ViewModel `M`
4. *However*, most ORMs support functionality to run those very "Search" operations on the database itself (Typically in the form of an `Expression<Func<E, bool>>`
5. *But* the fields for `M` may be complex derived properties that come from operations performed on fields from `E`

**The common solution often results in having to effectively define all these mappings in two directions**, once for mapping `E` -> `M` for the "read" operation, and a *second* time for mapping `M` properties and "translating" them into boolean "search" operations.

Which of course is a huge pain and adds a *lot* of extra boilerplate code, and furthermore introduces the pain point of having two sources of truth for the same operations.

If you modify how you map `E.SomeField` to `M.SomeOtherField`, you *also* need to remember to update the matching logic over in serializing your `SearchForM.SomeOtherField` DTO field into that `Expression<Func<M, bool>>`.

So, the question is as such:

1. Given:
  a. An existing defined `Expression<Func<E, M>>` that performs complex operations to derive "compound" or "complex" fields from E -> M
  b. A "Search" DTO that searches on the above given properties of `M`
2. Can we deconstruct the `Expression<Func<E, M>>` mapping expression tree, and reconstruct it into a `Expression<Func<E, bool>>` via the "Search" DTO

**The answer, it turns out, is Yes!**

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
}
```

Assuming we have an existing `IQueryable<Teacher> teachers` from our preferred ORM, and a `SearchQuery` passed in from some form of source (serializable POCO, you can accept it as a param on your API Endpoint(s) for example)

```
SearchQuery query = ...;
IQueryable<Teacher> teachers = ...;

Expression<Func<Teacher, TeacherDTO>> mapper = t => new ()
{
    Id = t.TeacherId,
    FullName = t.FirstName + " " + t.LastName
};

var compiledQuery = query.Compile(mapper);
var result = await teachers.SearchAsync(compiledQuery);

// 1 based inclusive index of the first item of the paged result (will just be 1 if no pagination was requested)
Console.WriteLine(result.From);
// 1 based inclusive index of the last item of the paged result (will be equal to "Of" if no pagination was requested)
Console.WriteLine(result.To);
// Total count of records after the search query filters but before pagination
Console.WriteLine(result.Of);

foreach (TeacherDTO dto in result.Results)
{
   ...
}
```

# How to install it

```
dotnet add package Nomadik.Core
```

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

# API
[See the Wiki for details on the application's API](https://github.com/SteffenBlake/Nomadik/wiki)
