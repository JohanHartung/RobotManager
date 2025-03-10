using RobotManager.Classes;

namespace RobotManager.Views;

public partial class AddClinicVisitPage : ContentPage
{
	Nao _nao;
	ClinicVisit _clinicVisit;

	List<Issue> selectedIssues = new();

    public AddClinicVisitPage(Nao nao, List<Issue> issues, ClinicVisit? clinicVisit = null, bool editMode = false)
    {
        InitializeComponent();
        BindingContext = nao;
        _nao = nao;
        _clinicVisit = clinicVisit ?? new ClinicVisit();
        IssueCV.ItemsSource = issues;
        selectedIssues = clinicVisit?.Issues.Select(i => issues.Find(issue => issue.Id == i)).ToList() ?? new List<Issue>();
    }

    private async void CreateVisit_Button_Clicked(object sender, EventArgs e)
    {
        _clinicVisit.Issues = selectedIssues.Select(i => i.Id).ToList();
        _clinicVisit.Nao = _nao.Id;
        _clinicVisit.Date = DateTime.Now;
        _clinicVisit.Notes = NotesEntry.Text;
        if(!await _clinicVisit.Post())
        {
            await DisplayAlert("Error", "Could not connect to the server ", "OK");
        }
        else
        {
            await _nao.SetStatus(Status.Clinic);
            await Navigation.PopAsync();
        }
    }

	private void IssueFrame_Tapped(object sender, TappedEventArgs e)
	{
		var frame = sender as Frame;
		var issue = frame.BindingContext as Issue;
		if (issue == null) { return; }
		Color normal = Color.FromArgb("#303030");
		Color selected = Color.FromArgb("#536578");

		if (selectedIssues.Contains(issue))
		{
			selectedIssues.Remove(issue);
			frame.BackgroundColor = normal;
		}
		else
		{
			selectedIssues.Add(issue);
			frame.BackgroundColor = selected;
		}
	}
}