using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using ModelContextProtocol.Server;

namespace McpDemo.Tools;

[McpServerToolType]
public static class TodoTool
{
    private static readonly object SyncRoot = new();
    private static readonly List<TodoItem> Items = new();

    [McpServerTool]
    [Description("列出目前記憶體中的待辦事項，可選擇是否包含已完成的項目。")]
    public static string ListTodos(bool includeCompleted = true)
    {
        List<TodoItem> snapshot;
        lock (SyncRoot)
        {
            snapshot = Items
                .Where(item => includeCompleted || !item.IsCompleted)
                .OrderBy(item => item.IsCompleted)
                .ThenBy(item => item.DueDate ?? DateTimeOffset.MaxValue)
                .ThenBy(item => item.CreatedAt)
                .ToList();
        }

        if (snapshot.Count == 0)
        {
            return includeCompleted
                ? "目前沒有任何待辦事項。"
                : "目前沒有未完成的待辦事項。";
        }

        var builder = new StringBuilder();
        foreach (var item in snapshot)
        {
            builder.Append("[");
            builder.Append(item.IsCompleted ? 'x' : ' ');
            builder.Append("] ");
            builder.Append(item.Title);

            if (item.DueDate is { } dueDate)
            {
                builder.Append(" (due: ");
                builder.Append(dueDate.ToLocalTime().ToString("g", CultureInfo.CurrentCulture));
                builder.Append(')');
            }

            builder.AppendLine();

            if (!string.IsNullOrWhiteSpace(item.Description))
            {
                builder.AppendLine($"    {item.Description}");
            }

            builder.AppendLine($"    id: {item.Id}");

            if (item.CompletedAt is { } completedAt)
            {
                builder.AppendLine($"    completed at: {completedAt.ToLocalTime():g}");
            }
        }

        return builder.ToString().TrimEnd();
    }

    [McpServerTool]
    [Description("新增一筆待辦事項並回傳指派的識別碼。")]
    public static string AddTodo(string title, string? description = null, DateTimeOffset? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("title 不能為空白。", nameof(title));
        }

        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            DueDate = dueDate,
            CreatedAt = DateTimeOffset.UtcNow
        };

        lock (SyncRoot)
        {
            Items.Add(item);
        }

        return $"已新增待辦事項 {item.Id}: {item.Title}";
    }

    [McpServerTool]
    [Description("將指定的待辦事項標記為完成。")]
    public static string CompleteTodo(string id)
    {
        if (!Guid.TryParse(id, out var todoId))
        {
            return $"識別碼 {id} 不是有效的 GUID。";
        }

        lock (SyncRoot)
        {
            var index = Items.FindIndex(item => item.Id == todoId);
            if (index < 0)
            {
                return $"找不到識別碼為 {id} 的待辦事項。";
            }

            var existing = Items[index];
            if (existing.IsCompleted)
            {
                return $"待辦事項 {id} 已標記為完成。";
            }

            Items[index] = existing with
            {
                IsCompleted = true,
                CompletedAt = DateTimeOffset.UtcNow
            };
        }

        return $"已完成待辦事項 {id}。";
    }

    [McpServerTool]
    [Description("清除所有已完成的待辦事項。")]
    public static string ClearCompleted()
    {
        int removed;
        lock (SyncRoot)
        {
            removed = Items.RemoveAll(item => item.IsCompleted);
        }

        return removed == 0
            ? "沒有已完成的待辦事項需要清除。"
            : $"已移除 {removed} 筆已完成的待辦事項。";
    }

    private record struct TodoItem
    {
        public Guid Id { get; init; }

        public required string Title { get; init; }

        public string? Description { get; init; }

        public DateTimeOffset? DueDate { get; init; }

        public bool IsCompleted { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset? CompletedAt { get; init; }
    }
}
