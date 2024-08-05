using FluentValidation.AspNetCore;
using MediatR;
using Npgsql;
using WebApplicationApi.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configura el contexto de la base de datos
builder.Services.AddSingleton<NpgsqlConnection>(sp => new NpgsqlConnection(builder.Configuration.GetConnectionString("ConexionDB")));
//add mediador
builder.Services.AddMediatR(typeof(AddNewCategory.Managment).Assembly);

//builder.Services.AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<AddNewCategory>());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
