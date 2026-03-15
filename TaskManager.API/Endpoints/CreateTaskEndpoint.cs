using FastEndpoints;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.API.Endpoints;

public class CreateTaskEndpoint : Endpoint<CreateTaskRequest, TaskResponse>
{
    private readonly ITaskRepository _repo;
    public CreateTaskEndpoint(ITaskRepository repo) => _repo = repo;

    public override void Configure()
    {
        Post("/tasks");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateTaskRequest req, CancellationToken ct)
    {
        var task = await _repo.CreateAsync(new TaskItem
        {
            Title = req.Title,
            Description = req.Description
        });

        await SendCreatedAtAsync<GetTasksEndpoint>(null, MapToResponse(task), cancellation: ct);
    }

    private static TaskResponse MapToResponse(TaskItem t) =>
        new(t.Id, t.Title, t.Description, t.Status, t.CreatedAt, t.UpdatedAt);
}
