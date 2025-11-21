using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using RobotManager.Classes;
namespace RobotManager.Views;

public partial class AddRobotPage : ContentPage
{
    Robot _robot;
    public AddRobotPage(Robot? robot = null, bool editMode = false)
    {
        InitializeComponent();
        _robot = robot ?? new Robot();

        if (editMode)
        {
            Title = $"Edit NAO{_robot.Name}";
            AddRobotButton.Text = "Save";
            NameEntry.Text = _robot.Name;
            HeadIdEntry.Text = _robot.HeadID;
            BodyIdEntry.Text = _robot.BodyID;
            PurchaseDatePicker.Date = _robot.Purchased;
            WarrantyExtensionEntry.Text = _robot.WarrantyExtension.ToString();
        }

    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        ResetDateTime();
    }

    private void ResetDateTime()
    {
        PurchaseDatePicker.Date = DateTime.Now;
    }

    private async void AddRobotButton_Clicked(object sender, EventArgs e)
    {
        int warranty = 0;
        try
        {
            warranty = int.Parse(WarrantyExtensionEntry.Text);
        }
        catch (Exception)
        {
            warranty = 0;
        }
        if (!FormIsValid()) { return; }

        _robot.Name = NameEntry.Text;
        _robot.HeadID = HeadIdEntry.Text;
        _robot.BodyID = BodyIdEntry.Text;
        _robot.Purchased = PurchaseDatePicker.Date.Value;
        _robot.WarrantyExtension = warranty;

        if(!await _robot.Post())
        {
            await DisplayAlert("Error", "Could not connect to the server ", "OK");
            return;
        }
        await Navigation.PopAsync();
    }

    private bool FormIsValid()
    {
        if (string.IsNullOrEmpty(NameEntry.Text))
        {
            DisplayAlert("Error", "Robot name is required", "OK");
            return false;
        }
        if (string.IsNullOrEmpty(HeadIdEntry.Text))
        {
            DisplayAlert("Error", "Head ID is required", "OK");
            return false;
        }
        if (string.IsNullOrEmpty(BodyIdEntry.Text))
        {
            DisplayAlert("Error", "Body ID is required", "OK");
            return false;
        }
        return true;
    }
}