using SQLite;
using ToDo.Models;

namespace ToDo.Data;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService()
    {
        // Шлях до файлу БД у AppDataDirectory
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "appdata.db3");
        _db = new SQLiteAsyncConnection(dbPath);

        // Створити таблицю, якщо ще нема
        _db.CreateTableAsync<TodoItem>().Wait();
    }

    // Отримати всі записи
    public Task<List<TodoItem>> GetItemsAsync() =>
        _db.Table<TodoItem>().ToListAsync();

    // Додати або оновити запис
    public Task<int> SaveItemAsync(TodoItem item) =>
        _db.InsertOrReplaceAsync(item);
    
    

    // Видалити запис
    public Task<int> DeleteItemAsync(TodoItem item) =>
        _db.DeleteAsync(item);
}