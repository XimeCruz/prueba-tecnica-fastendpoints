using FastEndpoints;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Interfaces;

namespace TaskManager.API.Endpoints;

public class MetricsEndpoint : EndpointWithoutRequest<MetricsResponse>
{
    private readonly ITaskRepository _repo;
    public MetricsEndpoint(ITaskRepository repo) => _repo = repo;

    public override void Configure()
    {
        Get("/metrics");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var (pending, inProgress, completed, total) = await _repo.GetMetricsAsync();
        await SendOkAsync(new MetricsResponse(pending, inProgress, completed, total), ct);
    }
}