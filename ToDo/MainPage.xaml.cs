using System.Diagnostics;
using ToDo.Data;

namespace ToDo;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _db;
    
    public MainPage(DatabaseService db)
    {
        InitializeComponent();
        Debug.WriteLine($"DB path: {FileSystem.AppDataDirectory}");
        _db = db;
    }

    private async void AddNewTaskButton_OnClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Name.Text)) DisplayAlert("Error", "Provide non null task name", "OK");
        if (string.IsNullOrEmpty(Description.Text)) DisplayAlert("Error", "Provide non null task name", "OK");

        var todoItem = new Models.TodoItem
        {
            Name = Name.Text,
            Description = Description.Text,
            Status = 0,
            DateCreated = DateTime.Now
        };
        await _db.SaveItemAsync(todoItem);
    }
}