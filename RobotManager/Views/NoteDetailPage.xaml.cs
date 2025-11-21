using RobotManager.Classes;

namespace RobotManager.Views;

public partial class NoteDetailPage : ContentPage
{
    Note _note;
    Robot _robot;
    
	public NoteDetailPage(Note note)
	{
        _note = note;
        _robot = new Robot();
        BindingContext = _note;
        InitializeComponent();
        InitalizeAsync();
    }

    private async Task InitalizeAsync()
    {
        await _robot.InitializeFromCloud(_note.Robot);
        Title = $"Issue #{_note.Id.ToString().PadLeft(4, '0')} | NAO{_robot.Name}";
    }

    private async void RemoveButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Delete", "Are you sure you want to delete this note?", "Yes", "No");
        if (!answer) { return; }

        if (!await _note.Delete())
        {
            await DisplayAlert("Error", "Note could not be deleted", "OK");
        }
        
    }

    private void OptionsButton_Clicked(object sender, EventArgs e)
    {
        OptionsInterface.IsVisible = !OptionsInterface.IsVisible;
        double currRot = OptionsButton.Rotation;
        OptionsButton.RotateTo(currRot == 0 ? 45 : 0);
    }

    private async void EditButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddNotePage(_robot, _note, true));
    }
}