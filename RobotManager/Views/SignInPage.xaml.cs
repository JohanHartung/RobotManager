using System.Security.Cryptography;
using System.Text;

namespace RobotManager.Views;

public partial class SignInPage : ContentPage
{
	List<Frame> EntryFrames;
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

    private void ContinueButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var frame = button!.Parent as Frame;
        if (frame == UsernameEtryForm)
        {
            bool existantUsername = true; // TODO: Check if username exists
            SetFrameVisibility(existantUsername ? PasswordEtryForm : RegistrationCodeEtryForm);
        }
        else if (frame == PasswordEtryForm)
        {
            string challenge = "1234"; // TODO: Get challenge from server
            string response = Hash(PasswordEntry1.Text, challenge);
            // TODO: Send response to server


        }
        else if (frame == RegistrationCodeEtryForm)
        {
            bool validCode = true; // TODO: Check if code is valid
            SetFrameVisibility(validCode ? RegistrationEtryForm : UsernameEtryForm);
        }
        else if (frame == RegistrationEtryForm)
        {
            string username = Hash(UsernameEntry2.Text);
            string password = Hash(PasswordEntry2.Text);
            string registrationCode = Hash(RegistrationCodeEntry.Text);
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