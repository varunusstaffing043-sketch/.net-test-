using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();   // # what we have changed
builder.Services.AddSwaggerGen();             // # what we have changed

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                         // # what we have changed
    app.UseSwaggerUI();                       // # what we have changed
}

app.MapGet("/health", () => Results.Ok("OK"))
   .WithName("HealthCheck")
   .WithOpenApi();                            // # what we have changed

var todos = new List<string> { "Buy milk", "Write tests" };

app.MapGet("/todo", () => Results.Ok(todos))
   .WithName("GetTodos")
   .WithOpenApi();                            // # what we have changed

app.MapPost("/todo", (string item) =>
{
    todos.Add(item);
    return Results.Created($"/todo/{todos.Count - 1}", item);
})
.WithName("AddTodo")
.WithOpenApi();                                // # what we have changed

app.Run();

