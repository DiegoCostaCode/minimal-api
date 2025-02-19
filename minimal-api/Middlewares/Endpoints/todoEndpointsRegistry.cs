using Microsoft.EntityFrameworkCore;
using minimal_api.DTOs;
using minimal_api.Infrastructure.Database;
using minimal_api.Models;

namespace minimal_api.Middlewares.Endpoints;

public static class TodoEndpointsRegistry
{
    //Método de extensão para registrar os endpoints de Todo na app
    public static void RegisterTodoEndpoints(this WebApplication app)
    {
        //Grupo de endpoints items Todo

        var todoItemsGroup = app.MapGroup("/todoitems");

        todoItemsGroup.MapGet("/", GetAllTodos);
        todoItemsGroup.MapGet("/complete", GetCompleteTodos);
        todoItemsGroup.MapGet("/{id}", GetTodo);
        todoItemsGroup.MapPost("/", CreateTodo);
        todoItemsGroup.MapPut("/{id}", UpdateTodo);
        todoItemsGroup.MapDelete("/{id}", DeleteTodo);
        
        //Métodos usando TypedResults
        
        static async Task<IResult> GetAllTodos(TodoDb db)
        {
            return TypedResults.Ok(await db.Todos.Select(todo => new TodoItemDTO(todo)).ToListAsync());
        }
        
        static async Task<IResult> GetCompleteTodos(TodoDb db) {
            return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(todo => new TodoItemDTO(todo)).ToListAsync());
        }
        
        static async Task<IResult> GetTodo(int id, TodoDb db)
        {
            return await db.Todos.FindAsync(id)
                is Todo todo
                ? TypedResults.Ok(new TodoItemDTO(todo))
                : TypedResults.NotFound();
        }
        
        static async Task<IResult> CreateTodo(TodoItemDTO todoItemDTO, TodoDb db)
        {
            var todoItem = new Todo
            {
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };
        
            db.Todos.Add(todoItem);
            await db.SaveChangesAsync();
        
            todoItemDTO = new TodoItemDTO(todoItem);
        
            return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
        }
        
        static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO, TodoDb db)
        {
            var todo = await db.Todos.FindAsync(id);
        
            if (todo is null) return TypedResults.NotFound();
        
            todo.Name = todoItemDTO.Name;
            todo.IsComplete = todoItemDTO.IsComplete;
        
            await db.SaveChangesAsync();
        
            return TypedResults.NoContent();
        }
        
        static async Task<IResult> DeleteTodo(int id, TodoDb db)
        {
            if (await db.Todos.FindAsync(id) is Todo todo)
            {
                db.Todos.Remove(todo);
                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }
        
            return TypedResults.NotFound();
        }
    }
}