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
}