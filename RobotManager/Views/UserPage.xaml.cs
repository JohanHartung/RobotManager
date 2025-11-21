using RobotManager.Classes;
using System.Security.Cryptography;
using System.Text;

namespace RobotManager.Views;

public partial class UserPage : ContentPage
{
	List<Frame> EntryFrames;

    User user = new();
    Button selectedTab;

    bool SignedIn = false;

    public UserPage()
	{
        //Preferences.Set("DeviceId", null);
        //Preferences.Set("Token", null);
        //Preferences.Set("UserId", -1);
        //Preferences.Set("UserName", null);

        SignedIn = user.Exists();

        InitializeComponent();
        InitUserPage(SignedIn);

		EntryFrames = new()
		{
			UsernameEtryForm,
			PasswordEtryForm,
			RegistrationCodeEtryForm,
			RegistrationEtryForm
		};
	}

	private void SetFrameVisibility(Frame frame)
    {
        foreach (var item in EntryFrames)
        {
            item.IsVisible = item == frame;
        }
    }

    private async void ContinueButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == UsernameEtryContinueButton)
        {
            user.Name = UsernameEntry1.Text;
            bool? existantUsername = await user.ValidUsername(user.Name);
            if(existantUsername == null)
            {
                await DisplayAlert("Error", "Could not connect to the server", "OK");
                return;
            }
            SetFrameVisibility((bool) existantUsername ? PasswordEtryForm : RegistrationCodeEtryForm);
        }
        else if (button == PasswordEntryContinueButton)
        {
            (int processId, string secret)? challenge = await user.LoginChallenge(user.Name);
            string response = Hash(PasswordEntry1.Text, challenge.Value.secret);
            var userdata = user.LoginResponse(challenge.Value.processId, response);
            user.Initialize(userdata.Result);
            // TODO: signin done
            SignedIn = true;
        }
        else if (button == RegistrationCodeEntryContinueButton)
        {
            bool validCode = await user.ValidRegistrationCode(RegistrationCodeEntry.Text);
            SetFrameVisibility(validCode ? RegistrationEtryForm : UsernameEtryForm);
        }
        else if (button == RegistrationEntryContinueButton)
        {
            string username = UsernameEntry2.Text;
            string password = Hash(PasswordEntry2.Text);
            var userdata = await user.CreateUser(username, password);
            user.Initialize(userdata);
            // TODO: registration done
            SignedIn = true;
            InitUserPage(SignedIn);
        }
    }
	private string Hash(string password, string? challenge = null)
    {
        using var sha256 = SHA256.Create();

        string clearText = challenge == null ? password : Hash(password) + challenge;
        var bytes = Encoding.UTF8.GetBytes(clearText);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }




    // toggles between the three tabs
    private void TabButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        selectedTab = button!;

        UpdateTabs();
    }

    private void UpdateTabs()
    {
        Color normal = Color.FromArgb("#303030");
        Dictionary<Button, (Color color, StackLayout tab)> config = new()
        {
            { NotesButton, (Color.FromArgb("#308a7b"), NoteLayout ) },
            { IssuesButton, (Color.FromArgb("#80464d"), IssueLayout ) },
            { ClinicButton, (Color.FromArgb("#80b2c9"), ClinicLayout ) }
        };

        foreach (var button in config.Keys)
        {
            if (button == selectedTab)
            {
                selectedTab.BackgroundColor = config[button].color;
                config[button].tab.IsVisible = true;
            }
            else
            {
                button.BackgroundColor = normal;
                config[button].tab.IsVisible = false;
            }
        }
    }

    private void InitUserPage(bool signedIn)
    {
        Title = signedIn ? "User" : "Sign In";
        SignInUI.IsVisible = !signedIn;
        UserUI.IsVisible = signedIn;

        if (signedIn)
        {
            UserNameLabel.Text = user.Name;
        }
    }

    private void Frame_Tapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        if (frame == null) { return; }

        Robot robot = new();

        if (frame.BindingContext.GetType() == typeof(Issue))
        {
            var issue = frame.BindingContext as Issue;
            Navigation.PushAsync(new IssueDetailPage(issue));
        }
        else if (frame.BindingContext.GetType() == typeof(Note))
        {
            var note = frame.BindingContext as Note;
            Navigation.PushAsync(new NoteDetailPage(note));
        }
        else
        {
            var clinicVisit = frame.BindingContext as ClinicVisit;
            Navigation.PushAsync(new ClinicVisitDetailPage(clinicVisit));
        }
    }

    private void SwitchGrid_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {

    }
}