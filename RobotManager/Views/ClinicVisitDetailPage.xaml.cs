using RobotManager.Classes;

namespace RobotManager.Views;

public partial class ClinicVisitDetailPage : ContentPage
{
    Nao _nao = new();
    ClinicVisit _visit;
    List<Issue>? _issues;
    public ClinicVisitDetailPage(ClinicVisit visit)
	{
		InitializeComponent();
		_visit = visit;
        BindingContext = _visit;
        _ = InitializeAsync();
    }
    private async Task InitializeAsync()
    {
        await _nao.InitializeFromCloud(_visit.Nao);
        _issues = new();

        _issues = _issues.Where(i => _visit.Issues.Contains(i.Id)).ToList();
        IssueCV.ItemsSource = _issues;
        Title = $"Clinic Visit #{_visit.Id.ToString().PadLeft(4, '0')} | NAO{_nao.Name}";
        NaoName.Text = $"NAO{_nao.Name}";
        NaoHead.Text = $"Head ID: {_nao.HeadID}";
        NaoBody.Text = $"Body ID: {_nao.BodyID}";
    }

    private async void ReturnedButton_Clicked(object sender, EventArgs e)
    {
		string backReport = await DisplayPromptAsync("Return Report", "Please enter a report:");
		bool issueSolved = await DisplayAlert("Issue(s) Solved?", "Mark issue(s) solved?", "Yes", "No");
        _visit.BackDate = DateTime.Now;
        _visit.BackReport = backReport;
        if (issueSolved && _issues != null)
        {
            foreach (var issue in _issues)
            {
                issue.Solved = (-2, DateTime.Now); // -2 => clinic
                issue.SolvedReport = $"Issue solved during clinic visit #{_visit.Id.ToString().PadLeft(4, '0')}";
                if (!await issue.SolveIssue())
                {
                    await DisplayAlert("Error", "Issue could not be updated", "OK");
                }
            }
        }

        if(!await _visit.EndClinicVisit())
        {
            await DisplayAlert("Error", "Clinic Visit could not be updated", "OK");
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
        // TODO: get global issues
        await Navigation.PushAsync(new AddClinicVisitPage(_nao, _issues!, _visit, true));
    }

    private async void RemoveButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Delete", "Are you sure you want to delete this clinic visit?", "Yes", "No");
        if (!answer) { return; }
        if (!await _visit.Delete())
        {
            await DisplayAlert("Error", "Clinic Visit could not be deleted", "OK");
        }
    }
}