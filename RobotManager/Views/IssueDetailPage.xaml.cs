using RobotManager.Classes;

namespace RobotManager.Views;

public partial class IssueDetailPage : ContentPage
{
    Issue _issue;
    Nao _nao;
    public IssueDetailPage(Issue issue, Nao nao)
	{
		InitializeComponent();

        _issue = issue;
        _nao = nao;
        BindingContext = _issue;
        Title = $"Issue #{_issue.Id.ToString().PadLeft(4, '0')} | NAO{_nao.Name}";
	}

    private void OptionsButton_Clicked(object sender, EventArgs e)
    {
        OptionsInterface.IsVisible = !OptionsInterface.IsVisible;
        double currRot = OptionsButton.Rotation;
        OptionsButton.RotateTo(currRot == 0 ? 45 : 0);
    }

    private async void ReplicateButton_Clicked(object sender, EventArgs e)
    {
        _issue.Replicated = true;
        if (!await _issue.Post())
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
        _issue.Solved = true;
        _issue.SolvedDate = DateTime.Now;
        _issue.SolvedReport = solvedReport;
        if (!await _issue.Post())
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
        await Navigation.PushAsync(new AddIssuePage(_nao, _issue, true));
    }
}