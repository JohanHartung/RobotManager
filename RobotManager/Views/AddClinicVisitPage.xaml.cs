using RobotManager.Classes;

namespace RobotManager.Views;

public partial class AddClinicVisitPage : ContentPage
{
	Nao _nao;
	ClinicVisit clinicVisit;

	List<Issue> selectedIssues = new();

	public AddClinicVisitPage(Nao nao, List<Issue> issues)
	{
		InitializeComponent();
		BindingContext = nao;
		_nao = nao;
		clinicVisit = new();
		IssueCV.ItemsSource = issues;
    }

    private void CreateVisit_Button_Clicked(object sender, EventArgs e)
    {
		//_nao.ClinicVisits.Add(_clinicVisit.Id);

		_nao.Status = Status.Clinic;
    }

	private void IssueFrame_Tapped(object sender, TappedEventArgs e)
	{
		var frame = sender as Frame;
		var issue = frame.BindingContext as Issue;
		if (issue == null) { return; }
		Color normal = Color.FromArgb("#303030");
		Color selected = Color.FromArgb("#455a64");

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