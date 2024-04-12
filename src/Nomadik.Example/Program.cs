using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Nomadik.Example;
using Nomadik.Example.Data;
using Nomadik.Example.Repositories;
using Nomadik.Extensions.Swagger;
using Nomadik.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Filters;
using Nomadik.Example.MapperProviders;
using Nomadik.Example.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ExampleDbContext>(options => 
{
    options.UseInMemoryDatabase(nameof(ExampleDbContext));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("Example", new()
    {
    });

    options.AddNomadik();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(
        Path.Combine(AppContext.BaseDirectory, xmlFilename)
    );

    options.ExampleFilters();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddNomadik(options =>
{
    options.AddProviderScoped<IndexTeachersProvider, Teacher, IndexTeachersDto>();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options => 
{
    options.SwaggerEndpoint("/swagger/Example/swagger.json", "Example");
    options.RoutePrefix = "";
});

app.MapPost("/teachers", TeacherController.Index)
.WithName("IndexTeachers");

Console.WriteLine("Loading names.txt into memory for seed data...");
var namesTxt = app.Environment.ContentRootFileProvider
    .GetFileInfo("names.txt"); 
var names = File.ReadLines(namesTxt.PhysicalPath!).ToArray();
Console.WriteLine("names.txt loaded. Seeding Database...");

using (var scope = app.Services.CreateScope())
{
    await using var db = 
        scope.ServiceProvider.GetRequiredService<ExampleDbContext>();
    await SeedData.Run(db, names);
}

Console.WriteLine("Database seeding complete.");

await app.RunAsync();
