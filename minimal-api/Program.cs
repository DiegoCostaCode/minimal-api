using Microsoft.EntityFrameworkCore;
using minimal_api;

var builder = WebApplication.CreateBuilder(args);

//Setando o banco de dados em memória
builder.Services.AddDbContext<TodoDb>(
    opt => opt.UseInMemoryDatabase("TodoList"));

//Promove logs de erro  no EF
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/todoitems", async (TodoDb todoDb) => 
    await todoDb.Todos.ToListAsync());

app.MapGet("/todoitems/complete", async (TodoDb db) =>
    await db.Todos.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id,TodoDb db) =>
    await db.Todos.FindAsync(id) 
        is Todo todo ? Results.Ok(todo) : Results.NotFound());

app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.Run();