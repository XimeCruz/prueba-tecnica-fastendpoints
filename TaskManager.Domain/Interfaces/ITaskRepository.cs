using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces;

public interface ITaskRepository
{
    Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem?> UpdateAsync(Guid id, TaskItem task);
    Task<bool> DeleteAsync(Guid id);
    Task<(int Pending, int InProgress, int Completed, int Total)> GetMetricsAsync();
}