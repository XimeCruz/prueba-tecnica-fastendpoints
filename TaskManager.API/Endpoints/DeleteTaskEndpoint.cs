using FastEndpoints;
using TaskManager.Domain.Interfaces;

namespace TaskManager.API.Endpoints;

public class DeleteTaskRequest { public Guid Id { get; set; } }

public class DeleteTaskEndpoint : Endpoint<DeleteTaskRequest>
{
    private readonly ITaskRepository _repo;
    public DeleteTaskEndpoint(ITaskRepository repo) => _repo = repo;

    public override void Configure()
    {
        Delete("/tasks/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteTaskRequest req, CancellationToken ct)
    {
        var deleted = await _repo.DeleteAsync(req.Id);
        if (!deleted) { await SendNotFoundAsync(ct); return; }
        await SendNoContentAsync();
    }
}