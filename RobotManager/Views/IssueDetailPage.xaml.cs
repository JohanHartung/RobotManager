using RobotManager.Classes;

namespace RobotManager.Views;

public partial class IssueDetailPage : ContentPage
{
	public IssueDetailPage(Issue issue)
	{
		InitializeComponent();

		BindingContext = issue;
	}
}