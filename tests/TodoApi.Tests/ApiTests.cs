using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { });
    }

    [Fact]
    public async Task Health_returns_ok()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/health");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Can_create_get_update_delete_todo()
    {
        var client = _factory.CreateClient();

        // Create
        var create = new { Title = "First", IsCompleted = false };
        var createdResp = await client.PostAsJsonAsync("/api/todos", create);
        createdResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createdResp.Content.ReadFromJsonAsync<TodoDto>();
        created.Should().NotBeNull();
        created!.Title.Should().Be("First");
        created.IsCompleted.Should().BeFalse();

        // Get by id
        var getResp = await client.GetAsync($"/api/todos/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.OK);

        // Update
        var update = new { Title = "First - Updated", IsCompleted = true };
        var updateResp = await client.PutAsJsonAsync($"/api/todos/{created.Id}", update);
        updateResp.StatusCode.Should().Be(HttpStatusCode.OK);

        // Delete
        var delResp = await client.DeleteAsync($"/api/todos/{created.Id}");
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify 404 after delete
        var get404 = await client.GetAsync($"/api/todos/{created.Id}");
        get404.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private record TodoDto(Guid Id, string Title, bool IsCompleted);
}
