using SQLite;

namespace ToDo.Models;

public class TodoItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public int Status { get; set; } // 0: Not Started, 1: In Progress, 2: Completed
    public DateTime DateCreated { get; set; }

    [Ignore]
    public string StatusIcon =>
        Status == 0 ? "⚫" :
        Status == 1 ? "⚪" 
        : "🟢";
}