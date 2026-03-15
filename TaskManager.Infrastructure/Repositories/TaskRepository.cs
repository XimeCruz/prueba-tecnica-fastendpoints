using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _db;

    public TaskRepository(AppDbContext db) => _db = db;

    public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var total = await _db.Tasks.CountAsync();
        var items = await _db.Tasks
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id) =>
        await _db.Tasks.FindAsync(id);

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        task.Status = TaskItemStatus.Pending;
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateAsync(Guid id, TaskItem updated)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task is null) return null;
        task.Status = updated.Status;
        task.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task is null) return false;
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<(int Pending, int InProgress, int Completed, int Total)> GetMetricsAsync()
    {
        var pending = await _db.Tasks.CountAsync(t => t.Status == TaskItemStatus.Pending);
        var inProgress = await _db.Tasks.CountAsync(t => t.Status == TaskItemStatus.InProgress);
        var completed = await _db.Tasks.CountAsync(t => t.Status == TaskItemStatus.Completed);
        return (pending, inProgress, completed, pending + inProgress + completed);
    }
}