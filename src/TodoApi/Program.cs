using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();           // # what we have changed: ensures endpoint metadata is discoverable
builder.Services.AddSwaggerGen();                     // # what we have changed: registers Swagger generator

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                                 // # what we have changed: enables OpenAPI document
    app.UseSwaggerUI();                               // # what we have changed: enables Swagger UI
}

// Health probe
app.MapGet("/health", () => Results.Ok("OK"))
   .WithName("HealthCheck")
   .WithOpenApi();                                    // # what we have changed: requires Microsoft.AspNetCore.OpenApi

// Sample in‑memory todo list
var todos = new List<string> { "Buy milk", "Write tests" };

app.MapGet("/todo", () => Results.Ok(todos))
   .WithName("GetTodos")
   .WithOpenApi();                                    // # what we have changed

app.MapPost("/todo", (string item) =>
{
    todos.Add(item);
    return Results.Created($"/todo/{todos.Count - 1}", item);
})
.WithName("AddTodo")
.WithOpenApi();                                       // # what we have changed

app.Run();
