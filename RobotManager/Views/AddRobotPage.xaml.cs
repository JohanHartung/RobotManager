using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using RobotManager.Classes;
namespace RobotManager.Views;

public partial class AddRobotPage : ContentPage
{
	public AddRobotPage()
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

    private void MoreButton_Clicked(object sender, EventArgs e)
    {
        MoreButton.IsVisible = false;
        MoreOptions.IsVisible = true;
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
        using HttpClient client = new();
        string apiUrl = "https://skakominor.de/api/RobotManager/CreateEdit/nao";

        HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, nao);

        if (response.IsSuccessStatusCode)
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