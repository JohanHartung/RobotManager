using RobotManager.Classes;

namespace RobotManager.Views;

public partial class AddGamePage : ContentPage
{
    private Game game = new();
	public AddGamePage()
	{
		InitializeComponent();
        ResetDateTime();
	}

    private void HomeCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var checkBox = sender as CheckBox;
        game.Home = checkBox!.IsChecked;
    }

    private async void CreateGameButton_Clicked(object sender, EventArgs e)
    {
        if (FormIsValid())
        {
            game.Against = OpponentEntry.Text;
            game.Date = IssueDatePicker.Date.Value.Add(IssueTimePicker.Time.Value);
            if (HomeCheckBox.IsChecked)
            {
                game.Home = HomeCheckBox.IsChecked;
            }

            if (!await game.Post())
            {
                await DisplayAlert("Error", "Could not connect to the server ", "OK");
            }
            else
            {
                await Navigation.PopAsync();
            }
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        ResetDateTime();
    }

    private void ResetDateTime()
    {
        IssueDatePicker.Date = DateTime.Now;
        IssueTimePicker.Time = DateTime.Now.TimeOfDay;
    }

    private bool FormIsValid()
    {
        if (string.IsNullOrEmpty(OpponentEntry.Text))
        {
            DisplayAlert("Error", "Opponent is required", "OK");
            return false;
        }
        return true;
    }

    private void picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = sender as Picker;
        game.Field = picker!.SelectedIndex switch
        {
            0 => Field.A,
            1 => Field.B,
            2 => Field.C,
            3 => Field.D,
            _ => Field.A
        };
    }
}