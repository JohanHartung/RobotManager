using RobotManager.Classes;

namespace RobotManager.Views;

public partial class ClinicVisitDetailPage : ContentPage
{
    Nao _nao;
    ClinicVisit _visit;
	List<Issue>? _issues;
    public ClinicVisitDetailPage(ClinicVisit visit, Nao nao, List<Issue> issues)
	{
		InitializeComponent();
        _nao = nao;
		_visit = visit;
        _issues = issues.Where(i => visit.Issues.Contains(i.Id)).ToList();
        BindingContext = _visit;
        IssueCV.ItemsSource = _issues;
        Title = $"Clinic Visit #{visit.Id.ToString().PadLeft(4, '0')} | NAO{nao.Name}";
		NaoName.Text = $"NAO{nao.Name}";
		NaoHead.Text = $"Head ID: {nao.HeadID}";
        NaoBody.Text = $"Body ID: {nao.BodyID}";

    }

    private async void ReturnedButton_Clicked(object sender, EventArgs e)
    {
		string backReport = await DisplayPromptAsync("Return Report", "Please enter a report:");
		bool issueSolved = await DisplayAlert("Issue(s) Solved?", "Mark issue(s) solved?", "Yes", "No");
		_visit.IsBack = true;
        _visit.BackDate = DateTime.Now;
        _visit.BackReport = backReport;
        if (issueSolved)
        {
            foreach (var issue in _issues)
            {
                issue.Solved = (-2, DateTime.Now); // -2 > clinic
                issue.SolvedReport = $"Issue solved during clinic visit #{_visit.Id.ToString().PadLeft(4, '0')}";
                if (!await issue.Post())
                {
                    await DisplayAlert("Error", "Issue could not be updated", "OK");
                }
            }
        }

        if(!await _visit.Post())
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