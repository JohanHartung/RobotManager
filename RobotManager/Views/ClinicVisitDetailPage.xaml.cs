using RobotManager.Classes;

namespace RobotManager.Views;

public partial class ClinicVisitDetailPage : ContentPage
{
	ClinicVisit _visit;
	List<Issue>? _issues;
    public ClinicVisitDetailPage(ClinicVisit visit, Nao nao, List<Issue> issues)
	{
		InitializeComponent();
		_visit = visit;
        _issues = issues.Where(i => visit.Issues.Contains(i.Id)).ToList();
        BindingContext = _visit;
        IssueCV.ItemsSource = _issues;
        Title = $"Clinic Visit #{visit.Id.ToString().PadLeft(4, '0')} | NAO{nao.Name}";
		NaoName.Text = $"NAO{nao.Name}";
		NaoHead.Text = $"Head ID: {nao.HeadID}";
        NaoBody.Text = $"Body ID: {nao.BodyID}";

    }
}