using RobotManager.Classes;

namespace RobotManager.Views;

public partial class ClinicVisitDetailPage : ContentPage
{
	public ClinicVisitDetailPage(ClinicVisit visit, Nao nao, List<Issue> issues)
	{
		InitializeComponent();
		BindingContext = visit;
		IssueCV.ItemsSource = issues.Where(i => visit.Issues.Contains(i.Id));
        Title = $"Issue #{visit.Id.ToString().PadLeft(4, '0')} | NAO{nao.Name}";
		NaoName.Text = $"NAO{nao.Name}";
		NaoHead.Text = $"Head ID: {nao.HeadID}";
        NaoBody.Text = $"Body ID: {nao.BodyID}";

    }
}