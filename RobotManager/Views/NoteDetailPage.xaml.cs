using RobotManager.Classes;

namespace RobotManager.Views;

public partial class NoteDetailPage : ContentPage
{
	public NoteDetailPage(Note note, Nao nao)
	{
		InitializeComponent();
        BindingContext = note;
        Title = $"Issue #{note.Id.ToString().PadLeft(4, '0')} | NAO{nao.Name}";
    }

    private void RemoveButton_Clicked(object sender, EventArgs e)
    {

    }

    private void OptionsButton_Clicked(object sender, EventArgs e)
    {
        OptionsInterface.IsVisible = !OptionsInterface.IsVisible;
        double currRot = OptionsButton.Rotation;
        OptionsButton.RotateTo(currRot == 0 ? 45 : 0);
    }

    private void EditButton_Clicked(object sender, EventArgs e)
    {

    }
}