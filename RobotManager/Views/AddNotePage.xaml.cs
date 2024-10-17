using RobotManager.Classes;
using System.Net.Http.Json;

namespace RobotManager.Views;

public partial class AddNotePage : ContentPage
{
	Nao _nao;
	Note note = new();
	public AddNotePage(Nao nao)
	{
		InitializeComponent();
        ResetDateTime();
        BindingContext = nao;
		_nao = nao;
    }

    private async void CreateNoteButton_Clicked(object sender, EventArgs e)
    {
        if (FormIsValid())
        {
            Note note = new()
            {
                Title = TitleEntry.Text,
                Date = NoteDatePicker.Date.Add(NoteTimePicker.Time),
                Description = NoteDescription.Text,
                Nao = _nao.Id
            };
            try
            {
                await PostNoteAsync(note);
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Could not connect to the server ", "OK");
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

    private async Task PostNoteAsync(Note note)
    {
        using HttpClient client = new();
        string apiUrl = "https://skakominor.de/api/RobotManager/CreateEdit/note";

        HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, note);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Success", "Note added successfully", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "Failed to add note", "OK");
        }
    }
}