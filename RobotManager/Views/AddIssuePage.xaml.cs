using RobotManager.Classes;
using System.Net.Http.Json;

namespace RobotManager.Views;

public partial class AddIssuePage : ContentPage
{
    Nao _nao;
    Issue _issue = new();
    bool _editMode;
    public AddIssuePage(Nao nao, Issue? issue = null, bool editMode = false)
    {
        InitializeComponent();
        ResetDateTime();
        BindingContext = nao;
        _nao = nao;
        _issue = issue ?? new Issue();
        _editMode = editMode;
        if (_editMode)
        {
            Title = $"Edit Issue #{_issue.Id.ToString().PadLeft(4, '0')} | NAO{_nao.Name}";
            CreateIssueButton.Text = "Save Changes";
            TitleEntry.Text = _issue.Title;
            IssueDatePicker.Date = _issue.Date;
            IssueTimePicker.Time = _issue.Date.TimeOfDay;
            IssueDescription.Text = _issue.Description;
            ReplicatedCheckBox.IsChecked = _issue.Replicated;
            ReplicatedDatePicker.Date = _issue.ReplicatedDate;
            ReplicatedTimePicker.Time = _issue.ReplicatedDate.TimeOfDay;
            SolvedCheckBox.IsChecked = _issue.Solved;
            SolvedDatePicker.Date = _issue.SolvedDate;
            SolvedTimePicker.Time = _issue.SolvedDate.TimeOfDay;
            SolvedReport.Text = _issue.SolvedReport;
        }

    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        ResetDateTime();
    }

    private async void CreateIssueButton_Clicked(object sender, EventArgs e)
    {
        if (FormIsValid())
        {
            _issue.Title = TitleEntry.Text;
            _issue.Date = IssueDatePicker.Date.Add(IssueTimePicker.Time);
            _issue.Description = IssueDescription.Text;
            _issue.Replicated = ReplicatedCheckBox.IsChecked;
            _issue.ReplicatedDate = ReplicatedDatePicker.Date.Add(ReplicatedTimePicker.Time);
            _issue.Solved = SolvedCheckBox.IsChecked;
            _issue.SolvedDate = SolvedDatePicker.Date.Add(SolvedTimePicker.Time);
            _issue.SolvedReport = SolvedReport.Text;
            _issue.Nao = _nao.Id;

            if (!await _issue.Post())
            {
                await DisplayAlert("Error", "Could not connect to the server ", "OK");
            }
        }
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