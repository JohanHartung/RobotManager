using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using RobotManager.Classes;
namespace RobotManager.Views;

public partial class AddRobotPage : ContentPage
{
    Nao _nao;
    public AddRobotPage(Nao? nao = null, bool editMode = false)
    {
        InitializeComponent();
        _nao = nao ?? new Nao();

        if (editMode)
        {
            Title = $"Edit NAO{_nao.Name}";
            AddRobotButton.Text = "Save";
            NameEntry.Text = _nao.Name;
            HeadIdEntry.Text = _nao.HeadID;
            BodyIdEntry.Text = _nao.BodyID;
            PurchaseDatePicker.Date = _nao.Purchased;
            WarrantyExtensionEntry.Text = _nao.WarrantyExtension.ToString();
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

        _nao.Name = NameEntry.Text;
        _nao.HeadID = HeadIdEntry.Text;
        _nao.BodyID = BodyIdEntry.Text;
        _nao.Purchased = PurchaseDatePicker.Date.Value;
        _nao.WarrantyExtension = warranty;

        if(!await _nao.Post())
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