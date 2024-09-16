using RobotManager.Classes;

namespace RobotManager.Views;

public partial class NotePage : ContentPage
{
	Nao _nao;
	Note _note = new();
	public NotePage(Nao nao, Note note, bool creatorMode = false)
	{
		InitializeComponent();
        ResetDateTime();
        BindingContext = nao;
		_nao = nao;
        _note = note;
    }

    private void CreateNoteButton_Clicked(object sender, EventArgs e)
    {

    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        ResetDateTime();
    }

    private void ResetDateTime()
    {
        NoteDatePicker.Date = DateTime.Now;
        NoteTimePicker.Time = DateTime.Now.TimeOfDay;
    }
}