using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health endpoint
app.MapGet("/health", () => Results.Ok("OK"))
   .WithName("HealthCheck")
   .WithOpenApi();

// Simple in-memory todos
var todos = new List<string> { "Buy milk", "Write tests" };

app.MapGet("/todo", () => Results.Ok(todos))
   .WithName("GetTodos")
   .WithOpenApi();

app.MapPost("/todo", (string item) =>
{
    todos.Add(item);
    return Results.Created($"/todo/{todos.Count - 1}", item);
})
.WithName("AddTodo")
.WithOpenApi();

// Prefer non-blocking shutdown
await app.RunAsync();

// Marker for WebApplicationFactory (non-static to be valid generic argument)
public partial class Program
{
    protected Program() { } // satisfies Sonar S1118
}
