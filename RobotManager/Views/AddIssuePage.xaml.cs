using RobotManager.Classes;
using System.Net.Http.Json;

namespace RobotManager.Views;

public partial class AddIssuePage : ContentPage
{
	Nao _nao;
	Issue _issue;
	public AddIssuePage(Nao nao, Issue issue , bool creatorMode = false)
	{
		InitializeComponent();
        ResetDateTime();
        BindingContext = nao;
		_nao = nao;
        _issue = issue;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        ResetDateTime();
    }

    private async void CreateIssueButton_Clicked(object sender, EventArgs e)
    {
        if (FormIsValid())
        {
            Issue issue = new()
            {
                Title = TitleEntry.Text,
                Date = IssueDatePicker.Date.Add(IssueTimePicker.Time),
                Description = IssueDescription.Text,
                Replicated = ReplicatedCheckBox.IsChecked,
                ReplicatedDate = ReplicatedDatePicker.Date.Add(ReplicatedTimePicker.Time),
                Solved = SolvedCheckBox.IsChecked,
                SolvedDate = SolvedDatePicker.Date.Add(SolvedTimePicker.Time),
                SolvedReport = SolvedReport.Text,
                Nao = _nao.Id
            };
            try
            {
                await PostIssueAsync(issue);
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Could not connect to the server ", "OK");
            }
        }
    }

    private void MoreButton_Clicked(object sender, EventArgs e)
    {
        IssueDescription.HeightRequest = 100;
        MoreButton.IsVisible = false;
        MoreOptions.IsVisible = true;
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var checkBox = sender as CheckBox;
        bool check;
        DateTime now = DateTime.Now;

        if (checkBox == ReplicatedCheckBox)
        {
            check = ReplicatedCheckBox.IsChecked;
            ReplicatedDateTime.IsVisible = check;

            ReplicatedDatePicker.Date = now;
            ReplicatedTimePicker.Time = now.TimeOfDay;
        }
        else if (checkBox == SolvedCheckBox)
        {
            check = SolvedCheckBox.IsChecked;
            SolvedDateTime.IsVisible = check;
            SolvedReport.IsVisible = check;

            SolvedDatePicker.Date = now;
            SolvedTimePicker.Time = now.TimeOfDay;

            SolvedReport.Text = "";
        }
    }

    private void ResetDateTime()
    {
        IssueDatePicker.Date = DateTime.Now;
        IssueTimePicker.Time = DateTime.Now.TimeOfDay;
    }

    private bool FormIsValid()
    {
        if (string.IsNullOrEmpty(TitleEntry.Text))
        {
            DisplayAlert("Error", "Issue title is required", "OK");
            return false;
        }
        if (string.IsNullOrEmpty(IssueDescription.Text))
        {
            DisplayAlert("Error", "Description is required", "OK");
            return false;
        }
        return true;
    }

    private async Task PostIssueAsync(Issue issue)
    {
        using HttpClient client = new();
        string apiUrl = "https://skakominor.de/api/RobotManager/CreateEdit/issue";

        HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, issue);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Success", "Issue added successfully", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "Failed to add issue", "OK");
        }
    }

}