using FluentValidation.AspNetCore;
using MediatR;
using Npgsql;
using WebApplicationApi.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configura el contexto de la base de datos
builder.Services.AddSingleton<NpgsqlConnection>(sp => new NpgsqlConnection(builder.Configuration.GetConnectionString("ConexionDB")));
//add mediador AddNewCategory
builder.Services.AddMediatR(typeof(AddNewCategory.Managment).Assembly);
// config servicios
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
//add map
builder.Services.AddAutoMapper(typeof(GetCategories.Managment));

// Configuración de servicios
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin() // Permitir cualquier origen
               .AllowAnyMethod() // Permitir cualquier método (GET, POST, etc.)
               .AllowAnyHeader(); // Permitir cualquier encabezado
    });
});

builder.Services.AddControllers(); // O cualquier otro servicio que necesites

//builder.Services.AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<AddNewCategory>());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
