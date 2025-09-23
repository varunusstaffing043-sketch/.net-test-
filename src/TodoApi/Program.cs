using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>(); // # Replace with real repository later

var app = builder.Build();

// Swagger in Dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Minimal API endpoints
var todos = app.MapGroup("/api/todos").WithTags("Todos");

todos.MapGet("/", (ITodoRepository repo) =>
{
    return Results.Ok(repo.GetAll());
})
.WithName("GetTodos")
.WithOpenApi();

todos.MapGet("/{id:guid}", (ITodoRepository repo, Guid id) =>
{
    var item = repo.Get(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
})
.WithName("GetTodoById")
.WithOpenApi();

todos.MapPost("/", (ITodoRepository repo, CreateTodoDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title)) return Results.BadRequest("Title is required.");
    var created = repo.Create(dto.Title, dto.IsCompleted);
    return Results.Created($"/api/todos/{created.Id}", created);
})
.WithName("CreateTodo")
.WithOpenApi();

todos.MapPut("/{id:guid}", (ITodoRepository repo, Guid id, UpdateTodoDto dto) =>
{
    var updated = repo.Update(id, dto.Title, dto.IsCompleted);
    return updated is not null ? Results.Ok(updated) : Results.NotFound();
})
.WithName("UpdateTodo")
.WithOpenApi();

todos.MapDelete("/{id:guid}", (ITodoRepository repo, Guid id) =>
{
    return repo.Delete(id) ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteTodo")
.WithOpenApi();

// Health endpoint
app.MapGet("/health", () => new { status = "ok", time = DateTimeOffset.UtcNow }) // # Extend with real health checks
.WithName("Health")
.WithOpenApi();

app.Run();

// Models and repository (kept simple for demo)
record Todo(Guid Id, string Title, bool IsCompleted);
record CreateTodoDto(string Title, bool IsCompleted);
record UpdateTodoDto(string Title, bool IsCompleted);

interface ITodoRepository
{
    IEnumerable<Todo> GetAll();
    Todo? Get(Guid id);
    Todo Create(string title, bool isCompleted = false);
    Todo? Update(Guid id, string title, bool isCompleted);
    bool Delete(Guid id);
}

class InMemoryTodoRepository : ITodoRepository
{
    private readonly Dictionary<Guid, Todo> _items = new();

    public IEnumerable<Todo> GetAll() => _items.Values.OrderBy(t => t.Title);
    public Todo? Get(Guid id) => _items.TryGetValue(id, out var t) ? t : null;

    public Todo Create(string title, bool isCompleted = false)
    {
        var todo = new Todo(Guid.NewGuid(), title, isCompleted);
        _items[todo.Id] = todo;
        return todo;
    }

    public Todo? Update(Guid id, string title, bool isCompleted)
    {
        if (!_items.ContainsKey(id)) return null;
        var updated = new Todo(id, title, isCompleted);
        _items[id] = updated;
        return updated;
    }

    public bool Delete(Guid id) => _items.Remove(id);
}
