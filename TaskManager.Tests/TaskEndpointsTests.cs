using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.DTOs;
using TaskManager.Infrastructure.Persistence;
using Xunit;
using FluentAssertions;

namespace TaskManager.Tests;

public class TaskEndpointsTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public TaskEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace DB with test DB
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(opts =>
                    opts.UseNpgsql("Host=localhost;Database=taskmanager_test;Username=postgres;Password=postgresql"));
            });
        });
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
        await db.Tasks.ExecuteDeleteAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task CreateTask_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/tasks", new { Title = "Test Task", Description = "Desc" });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();
        task!.Title.Should().Be("Test Task");
    }

    [Fact]
    public async Task GetTasks_ReturnsPaginatedResults()
    {
        await _client.PostAsJsonAsync("/tasks", new { Title = "Task 1", Description = "" });
        await _client.PostAsJsonAsync("/tasks", new { Title = "Task 2", Description = "" });

        var request = new HttpRequestMessage(HttpMethod.Get, "/tasks");
        request.Headers.Add("X-Page", "1");
        request.Headers.Add("X-PageSize", "1");

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("X-TotalCount");
        response.Headers.Should().ContainKey("X-TotalPages");
    }

    [Fact]
    public async Task CreateTask_WithEmptyTitle_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/tasks", new { Title = "", Description = "Desc" });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNoContent()
    {
        var created = await _client.PostAsJsonAsync("/tasks", new { Title = "To Delete", Description = "" });
        var task = await created.Content.ReadFromJsonAsync<TaskResponse>();

        var response = await _client.DeleteAsync($"/tasks/{task!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetMetrics_ReturnsStats()
    {
        await _client.PostAsJsonAsync("/tasks", new { Title = "Task A", Description = "" });
        var response = await _client.GetAsync("/metrics");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var metrics = await response.Content.ReadFromJsonAsync<MetricsResponse>();
        metrics!.Total.Should().BeGreaterThan(0);
    }
}