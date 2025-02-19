using System.Collections;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using minimal_api;
using minimal_api.Infrastructure.Database;
using minimal_api.Middlewares.Endpoints;

var builder = WebApplication.CreateBuilder(args);

//Identifica se o ambiente é de desenvolvimento
if (builder.Environment.IsDevelopment())
{
    //Configura a serialização de JSON. Recomenda-se para ambiente de desenvolvimento por gasto de processamento.
    builder.Services.ConfigureHttpJsonOptions(opt =>
    {
        opt.SerializerOptions.WriteIndented = true;
        opt.SerializerOptions.WriteIndented = true;
    });
}

//Setando o banco de dados em memória
builder.Services.AddDbContext<TodoDb>(
    opt => opt.UseInMemoryDatabase("TodoList"));

//Promove logs de erro  no EF
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

//URL HelloWorld
app.MapGet("/", () => "Hello World!");

//Registra os endpoints de Todo no app
app.RegisterTodoEndpoints();

app.Run();

