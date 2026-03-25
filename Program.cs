using System.Reflection;
using CqrsApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CQRS API",
        Version = "v1",
        Description = "A .NET Core Web API using CQRS pattern with MediatR and Dapper"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// Add MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Add Dapper context
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddSingleton<DatabaseInitializer>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await dbInitializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CQRS API v1");
    options.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();
