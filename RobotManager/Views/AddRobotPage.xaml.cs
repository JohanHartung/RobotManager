using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using RobotManager.Classes;
namespace RobotManager.Views;

public partial class AddRobotPage : ContentPage
{
    Nao _nao;
	public AddRobotPage(Nao? nao = null)
	{
		InitializeComponent();
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
        if (FormIsValid())
        {
            Nao nao = new()
            {
                Name = NameEntry.Text,
                HeadID = HeadIdEntry.Text,
                BodyID = BodyIdEntry.Text,
                Purchased = PurchaseDatePicker.Date,
                WarrantyExtension = warranty
            };
            try
            {
                await PostNaoAsync(nao);
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Could not connect to the server ", "OK");
            }
        }
    }
    private async Task PostNaoAsync(Nao nao)
    {
        
        if (await nao.Post())
        {
            await DisplayAlert("Success", "Robot added successfully", "OK");
            Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "Failed to add robot", "OK");
        }
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