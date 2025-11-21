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
            Title = $"Edit NAO{_robot.HeadNumber}";
            AddRobotButton.Text = "Save";
            NameEntry.Text = _robot.HeadNumber.ToString();
            HeadIdEntry.Text = _robot.HeadSerial;
            BodyIdEntry.Text = _robot.BodySerial;
            PurchaseDatePicker.Date = _robot.Purchased;
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

        _robot.HeadNumber = Convert.ToInt32(NameEntry.Text);
        _robot.HeadSerial = HeadIdEntry.Text;
        _robot.BodySerial = BodyIdEntry.Text;
        _robot.Purchased = PurchaseDatePicker.Date.Value;

        //if(!await _robot.Post())
        //{
        //    await DisplayAlert("Error", "Could not connect to the server ", "OK");
        //    return;
        //}
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