using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ToDo.Data;
using ToDo.Models;

namespace ToDo.Views;

public partial class TasksPage : ContentPage, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private readonly DatabaseService _db;
    
    public ObservableCollection<TodoItem> Items { get; } = new();
    public ObservableCollection<TodoItem> FilteredItems { get; } = new();
    
    private TodoItem _chosenItem;
    public TodoItem ChosenItem
    {
        get => _chosenItem;
        set
        {
            _chosenItem = value;
            OnPropertyChanged(nameof(ChosenItem)); // 🔥 CRUCIAL
        }
    }
    
    private void ApplyFilter(int status)
    {
        FilteredItems.Clear();

        IEnumerable<TodoItem> filtered;

        if (status == -1)
            filtered = Items;  // Всі
        else
            filtered = Items.Where(i => i.Status == status);

        foreach (var item in filtered)
            FilteredItems.Add(item);
    }

    
    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    
    public TasksPage(DatabaseService db)
    {
        InitializeComponent();
        BindingContext = this;
        _db = db;
        LoadItems();
    }
    
    
    private async void LoadItems()
    {
        var list = await _db.GetItemsAsync();
        Items.Clear();
        foreach (var i in list)
            Items.Add(i);

        ApplyFilter(-1);
        FilteredItems.Clear();
        foreach (var item in Items)
            FilteredItems.Add(item);
    }
    
    private async void FiltersButton_OnClicked(object? sender, EventArgs e)
    {
        string action = await DisplayActionSheet("Фільтр за статусом", "Відміна", null, "Всі", "0", "1", "2");

        if (action == null || action == "Відміна")
            return;

        if (action == "Всі")
        {
            ApplyFilter(-1);
        }
        else if (int.TryParse(action, out int status))
        {
            ApplyFilter(status);
        }
    }

    private void TaskEntry_OnCompleted(object? sender, EventArgs e)
    {
        string filter = TaskEntry.Text.ToLower() ?? string.Empty;

        FilteredItems.Clear();

        var filtered = Items.Where(item =>
            (!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(filter))
            || (!string.IsNullOrEmpty(item.Description) && item.Description.ToLower().Contains(filter)));

        foreach (var item in filtered)
            FilteredItems.Add(item);
    }

    private void RefreshButton_OnClicked(object? sender, EventArgs e)
    {
        LoadItems();
        
        if (ItemDetailsFlexLayout.IsVisible)
        {
            ItemDetailsFlexLayout.IsVisible = false;
            ChosenItem = null;
        }
    }

    private void ItemButton_OnClicked(object? sender, EventArgs e)
    {
        ItemDetailsFlexLayout.IsVisible = !ItemDetailsFlexLayout.IsVisible;

        ChosenItem = null;
        
        if (sender is Button button && button.BindingContext is TodoItem item)
        {
            ChosenItem = item;
            Debug.WriteLine($"Chosen item: {ChosenItem.Name} - {ChosenItem.Description} - {ChosenItem.Status} - {ChosenItem.DateCreated}");
        }
    }

    private void DeleteButton_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            _db.DeleteItemAsync(ChosenItem);
            DisplayAlert("Success", "Item deleted successfully.", "OK");
            ItemDetailsFlexLayout.IsVisible = !ItemDetailsFlexLayout.IsVisible;
        } catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting item: {ex.Message}");
            DisplayAlert("Error", "Failed to delete item.", "OK");
        }
    }

    private async void Picker_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        var picker = (Picker)sender;
        if (picker.SelectedItem is string selectedIcon)
        {
            var item = (TodoItem)picker.BindingContext;
            if (selectedIcon == "⚫") item.Status = 0;
            else if (selectedIcon == "⚪") item.Status = 1;
            else if (selectedIcon == "🟢") item.Status = 2;
            await _db.SaveItemAsync(item);
        }
    }

    private async void ButtonEdit_OnClicked(object? sender, EventArgs e)
    {
        if (ChosenItem == null)
        {
            await DisplayAlert("Помилка", "Виберіть завдання для редагування.", "OK");
            return;
        }

        // Запит нового імені
        string newName = await DisplayPromptAsync("Редагувати Назву", "Введіть нову назву:", initialValue: ChosenItem.Name);
        if (string.IsNullOrWhiteSpace(newName))
        {
            // Можна або вивести повідомлення, або просто скасувати
            await DisplayAlert("Помилка", "Назва не може бути порожньою.", "OK");
            return;
        }

        // Запит нового опису
        string newDescription = await DisplayPromptAsync("Редагувати Опис", "Введіть новий опис:", initialValue: ChosenItem.Description);

        // Оновлюємо модель
        ChosenItem.Name = newName;
        ChosenItem.Description = newDescription;

        // Зберігаємо в базу
        await _db.SaveItemAsync(ChosenItem);

        // Оновлюємо UI
        OnPropertyChanged(nameof(ChosenItem));

        await DisplayAlert("Успіх", "Зміни збережено", "OK");
    }
}