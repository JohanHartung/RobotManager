using RobotManager.Classes;

namespace RobotManager.Views;

public partial class IssueDetailPage : ContentPage
{
    Issue _issue;
    Robot _robot;
    public IssueDetailPage(Issue issue)
	{
        _issue = issue;
        _robot = new Robot();
        BindingContext = _issue;
        InitializeComponent();
        _ = InitializeAsync();
	}

    private async Task InitializeAsync()
    {
        await _robot.InitializeFromCloud(_issue.Robot);
        Title = $"Issue #{_issue.Id.ToString().PadLeft(4, '0')} | NAO{_robot.HeadNumber}";
    }

    private void OptionsButton_Clicked(object sender, EventArgs e)
    {
        OptionsInterface.IsVisible = !OptionsInterface.IsVisible;
        double currRot = OptionsButton.Rotation;
        OptionsButton.RotateTo(currRot == 0 ? 45 : 0);
    }

    private async void ReplicateButton_Clicked(object sender, EventArgs e)
    {
        _issue.Replicated.Add(Preferences.Get("userID", -1), DateTime.Now);
        if (!await _issue.ReplicateIssue())
        {
            await DisplayAlert("Error", "Could not connect to the server", "OK");
        }
        
    }

    private async void SolveButton_Clicked(object sender, EventArgs e)
    {
        string solvedReport = await DisplayPromptAsync("Solved Report", "If issue could not be replicated, leave empty");
        if (solvedReport == null)
        {
            // User pressed Cancel
            // use string.IsNullOrWhiteSpace(solvedReport) to check if user pressed OK with empty input
            return;
        }
        _issue.Solved = (Preferences.Get("userID", -1), DateTime.Now);
        _issue.SolvedReport = solvedReport;
        if (!await _issue.SolveIssue())
        {
            await DisplayAlert("Error", "Could not connect to the server", "OK");
        }
    }

    private async void RemoveButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Delete", "Are you sure you want to delete this issue?", "Yes", "No");
        if (!answer) { return; }
        if (!await _issue.Delete())
        {
            await DisplayAlert("Error", "Issue could not be deleted", "OK");
        }
    }

    private async void EditButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddIssuePage(_robot, _issue, true));
    }
}