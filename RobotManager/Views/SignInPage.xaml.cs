using RobotManager.Classes;
using System.Security.Cryptography;
using System.Text;

namespace RobotManager.Views;

public partial class SignInPage : ContentPage
{
	List<Frame> EntryFrames;

    User user = new();

    public SignInPage()
	{
		InitializeComponent();
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
        var frame = button!.Parent as Frame;
        if (frame == UsernameEtryForm)
        {
            user.Name = UsernameEntry1.Text;
            bool existantUsername = await user.ValidUsername(user.Name);
            SetFrameVisibility(existantUsername ? PasswordEtryForm : RegistrationCodeEtryForm);
        }
        else if (frame == PasswordEtryForm)
        {
            (int processId, string secret)? challenge = await user.LoginChallenge(user.Name);
            string response = Hash(PasswordEntry1.Text, challenge.Value.secret);
            var userdata = user.LoginResponse(challenge.Value.processId, response);
            user.Initialize(userdata.Result);
            // TODO: signin done
        }
        else if (frame == RegistrationCodeEtryForm)
        {
            bool validCode = await user.ValidRegistrationCode(RegistrationCodeEntry.Text);
            SetFrameVisibility(validCode ? RegistrationEtryForm : UsernameEtryForm);
        }
        else if (frame == RegistrationEtryForm)
        {
            string username = UsernameEntry2.Text;
            string password = Hash(PasswordEntry2.Text);
            var userdata = await user.CreateUser(username, password);
            user.Initialize(userdata);
            // TODO: registration done
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
}