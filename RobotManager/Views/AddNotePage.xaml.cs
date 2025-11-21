using RobotManager.Classes;
using System.Net.Http.Json;

namespace RobotManager.Views;

public partial class AddNotePage : ContentPage
{
    Nao _nao;
    Note _note;
    public AddNotePage(Nao nao, Note? note = null, bool editMode = false)
    {
        InitializeComponent();
        ResetDateTime();

        _nao = nao;
        _note = note ?? new Note();

        if (editMode)
        {
            Title = $"Edit Note #{_note.Id.ToString().PadLeft(4, '0')} | NAO{_nao.Name}";
            CreateNoteButton.Text = "Save Changes";
            TitleEntry.Text = _note.Title;
            NoteDatePicker.Date = _note.Date;
            NoteTimePicker.Time = _note.Date.TimeOfDay;
            NoteDescription.Text = _note.Description;
        }

        BindingContext = _nao;
    }

    private async void CreateNoteButton_Clicked(object sender, EventArgs e)
    {
        if (FormIsValid())
        {

            _note.Title = TitleEntry.Text;
            _note.Date = NoteDatePicker.Date.Value.Add(NoteTimePicker.Time.Value);
            _note.Description = NoteDescription.Text;
            _note.Nao = _nao.Id;
            _note.Author = Preferences.Get("UserId", -1);

            if (!await _note.Post())
            {
                await DisplayAlert("Error", "Could not connect to the server ", "OK");
            }
            else
            {
                await DisplayAlert("Succes", "should be done", "OK");
                //await Navigation.PopAsync();
            }
        }
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

    private bool FormIsValid()
    {
        if (string.IsNullOrEmpty(TitleEntry.Text))
        {
            DisplayAlert("Error", "Note title is required", "OK");
            return false;
        }
        if (string.IsNullOrEmpty(NoteDescription.Text))
        {
            DisplayAlert("Error", "Description is required", "OK");
            return false;
        }
        return true;
    }
}