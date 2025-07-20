using Application.Interfaces;
using Infrastructure.Cadastros;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using System.Reflection;
using Application.Cadastros;
using DotNetEnv;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Oficina Mecânica API", 
        Version = "v1",
        Description = "API para gerenciamento de oficina mecânica",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "João Dainese",
            Email = "joaosenadainese@gmail.com"
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configure Entity Framework Core with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register application services
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Oficina Mecânica API v1");
        c.RoutePrefix = string.Empty; // Makes Swagger UI available at app root
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
    //remove dbcontext hereto avoid issues with multiple instances
    public static void RemoveDbContext(IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(AppDbContext));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
        var optionsDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
        if (optionsDescriptor != null)
        {
            services.Remove(optionsDescriptor);
        }
    }
}