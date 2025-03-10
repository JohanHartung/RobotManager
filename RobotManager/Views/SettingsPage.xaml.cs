

namespace RobotManager.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
        uriText.Text = Preferences.Get("uri", "https://example.com/api/RobotManager/");
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        uriText.Text = $"http://{domainEntry.Text}/api/RobotManager/";
        //Preferences.Set("uri", domainEntry.Text);
        Preferences.Set("uri", $"http://{domainEntry.Text}/api/RobotManager/");
    }

    private void domainEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        uriText.Text = $"http://{domainEntry.Text}/api/RobotManager/";
    }

    private async void ResetButton_Clicked(object sender, EventArgs e)
    {
        bool choice = await DisplayAlert("Reset User Data", "Are you sure you want to reset the user data?", "Yes", "No");
        if (choice)
        {
            Preferences.Set("DeviceId", null);
            Preferences.Set("Token", null);
            Preferences.Set("UserId", -1);
            Preferences.Set("UserName", null);
        }
    }
}