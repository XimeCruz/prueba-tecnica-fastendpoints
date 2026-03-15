using TaskManager.Domain.Entities;

namespace TaskManager.Application.DTOs;

public record CreateTaskRequest(string Title, string Description);

public record UpdateTaskRequest(TaskItemStatus Status);

public record TaskResponse(
    Guid Id,
    string Title,
    string Description,
    TaskItemStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record MetricsResponse(int Pending, int InProgress, int Completed, int Total);