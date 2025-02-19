using Microsoft.EntityFrameworkCore;
using minimal_api.Models;

namespace minimal_api.Infrastructure.Database;

public class TodoDb:DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options) : base(options)
    {}

    public DbSet<Todo> Todos => Set<Todo>();
}