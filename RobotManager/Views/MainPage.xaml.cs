namespace RobotManager.Views;
using static Classes.Helpers;
using Classes;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
#if DEBUG
        resetButton.IsVisible = true;
        loginButton.IsVisible = true;
#endif
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("DeviceId", null);
        Preferences.Set("Token", null);
        Preferences.Set("UserId", -1);
        Preferences.Set("UserName", null);
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        User user = new();
        string username = "Johan";
        string password = Hash("1357");
        var userdata = await user.CreateUser(username, password);
        user.Initialize(userdata);
    }
}


