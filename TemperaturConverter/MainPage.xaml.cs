using System.Diagnostics;

namespace TemperaturConverter;

public partial class MainPage : ContentPage
{

    public double Celsius { get; set; }
    public string ChosenOutput { get; set; }
    public double Fahrenheit => (Celsius * 9 / 5) + 32;
    public double Kelvin => Celsius + 273.15;
    
    public MainPage()
    {
        InitializeComponent();
    }

    private void ConverterEntry_OnCompleted(object? sender, EventArgs e)
    {
        if (!int.TryParse(ConverterEntry.Text, out int value))  DisplayAlert("Error", "Please enter a valid number.", "OK");

        Celsius = int.Parse(ConverterEntry.Text);
        
        if (ChosenOutput == "") DisplayAlert("Error", "Please select a unit.", "OK");
        if (ChosenOutput == "F") ConverterOutput.Text = Fahrenheit.ToString();
        else if (ChosenOutput == "K") ConverterOutput.Text = Kelvin.ToString();
        else DisplayAlert("Error", "Please select a valid unit.", "OK");
        Debug.WriteLine("Chosen unit: " + ChosenOutput);

        ChosenOutput = "";

    }

    private void RadioButton_OnCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        RadioButton rb = sender as RadioButton;
        if (rb == null) return;

        if (e.Value)
        {
            if (rb.Content.ToString() == "Fahrenheit")
            {
                ChosenOutput = "F";
            }
            else if (rb.Content.ToString() == "Kelvin")
            {
                ChosenOutput = "K";
            }
            Debug.WriteLine("Chosen via Content: " + ChosenOutput);
        }
    }
}