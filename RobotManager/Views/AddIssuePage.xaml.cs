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
            if (_issue.IsReplicated)
            {
                ReplicatedCheckBox.IsChecked = _issue.IsReplicated;
                ReplicatedDatePicker.Date = _issue.Replicated.Values.Min().Date;
                ReplicatedTimePicker.Time = _issue.Replicated.Values.Min().TimeOfDay;
            }
            if (_issue.IsSolved)
            {
                SolvedCheckBox.IsChecked = _issue.IsSolved;
                SolvedDatePicker.Date = _issue.Solved.Value.dateTime.Date;
                SolvedTimePicker.Time = _issue.Solved.Value.dateTime.TimeOfDay;
                SolvedReport.Text = _issue.SolvedReport;
            }
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
            _issue.Date = IssueDatePicker.Date.Value.Add(IssueTimePicker.Time.Value);
            _issue.Description = IssueDescription.Text;
            if (ReplicatedCheckBox.IsChecked)
            {
                _issue.Replicated!.Add(Preferences.Get("userID", -1), ReplicatedDatePicker.Date.Value.Add(ReplicatedTimePicker.Time.Value));
            }
            if(SolvedCheckBox.IsChecked)
            {
                _issue.Solved = new() { user = Preferences.Get("userID", -1), dateTime = SolvedDatePicker.Date.Value.Add(SolvedTimePicker.Time.Value) };
                _issue.SolvedReport = SolvedReport.Text;
            }
            _issue.Nao = _nao.Id;

            if (!await _issue.Post())
            {
                await DisplayAlert("Error", "Could not connect to the server ", "OK");
            }
            else
            {
                await Navigation.PopAsync();
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

}