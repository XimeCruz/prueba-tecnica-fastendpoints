using FastEndpoints;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.API.Endpoints;

public class UpdateTaskRequest2
{
    public Guid Id { get; set; }
    public TaskItemStatus Status { get; set; }
}

public class UpdateTaskEndpoint : Endpoint<UpdateTaskRequest2, TaskResponse>
{
    private readonly ITaskRepository _repo;
    public UpdateTaskEndpoint(ITaskRepository repo) => _repo = repo;

    public override void Configure()
    {
        Put("/tasks/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateTaskRequest2 req, CancellationToken ct)
    {
        var updated = await _repo.UpdateAsync(req.Id, new TaskItem { Status = req.Status });
        if (updated is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendOkAsync(new TaskResponse(updated.Id, updated.Title, updated.Description, updated.Status, updated.CreatedAt, updated.UpdatedAt), ct);
    }
}