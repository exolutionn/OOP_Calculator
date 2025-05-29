namespace ExpensesCalculator;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Clicked(object sender, EventArgs e)
    {
        bool isValid = int.TryParse(PersonsEntry.Text, out int persons) && persons > 0;
        isValid &= double.TryParse(DistanceEntry.Text, out double distance) && distance >= 0;
        isValid &= double.TryParse(FuelCostEntry.Text, out double fuelCost) && fuelCost >= 0;
        isValid &= double.TryParse(FuelConsumptionEntry.Text, out double fuelConsumption) && fuelConsumption >= 0;
        isValid &= double.TryParse(AccommodationCostEntry.Text, out double accommodationCost) && accommodationCost >= 0;
        isValid &= int.TryParse(NightsEntry.Text, out int nights) && nights >= 0;

        if (!isValid)
        {
            ResultLabel.TextColor = Colors.Red;
            ResultLabel.Text = "Будь ласка, введіть коректні числові значення у всі поля.";
            return;
        }

        double fuelExpenses = (distance / 100) * fuelConsumption * fuelCost;
        double accommodationExpenses = persons * nights * accommodationCost;
        double totalExpenses = fuelExpenses + accommodationExpenses;
        double perPerson = totalExpenses / persons;

        ResultLabel.TextColor = Colors.DarkGreen;
        ResultLabel.Text = $"Загальні витрати: {totalExpenses:F2} грн\n" +
                           $"Витрати на паливо: {fuelExpenses:F2} грн\n" +
                           $"Витрати на проживання: {accommodationExpenses:F2} грн\n" +
                           $"Витрати на одну особу: {perPerson:F2} грн";
    }
}