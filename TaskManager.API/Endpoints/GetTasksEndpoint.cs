using FastEndpoints;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Interfaces;

namespace TaskManager.API.Endpoints;

public class GetTasksEndpoint : EndpointWithoutRequest<IEnumerable<TaskResponse>>
{
    private readonly ITaskRepository _repo;
    public GetTasksEndpoint(ITaskRepository repo) => _repo = repo;

    public override void Configure()
    {
        Get("/tasks");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var page = int.TryParse(HttpContext.Request.Headers["X-Page"], out var p) ? p : 1;
        var pageSize = int.TryParse(HttpContext.Request.Headers["X-PageSize"], out var ps) ? ps : 10;

        var (items, total) = await _repo.GetAllAsync(page, pageSize);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        HttpContext.Response.Headers["X-TotalCount"] = total.ToString();
        HttpContext.Response.Headers["X-TotalPages"] = totalPages.ToString();

        await SendOkAsync(items.Select(t =>
            new TaskResponse(t.Id, t.Title, t.Description, t.Status, t.CreatedAt, t.UpdatedAt)), ct);
    }
}