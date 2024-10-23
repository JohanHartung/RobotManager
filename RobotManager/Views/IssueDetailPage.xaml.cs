using RobotManager.Classes;

namespace RobotManager.Views;

public partial class IssueDetailPage : ContentPage
{
	public IssueDetailPage(Issue issue, Nao nao)
	{
		InitializeComponent();

		BindingContext = issue;
        Title = $"Issue #{issue.Id.ToString().PadLeft(4, '0')} | NAO{nao.Name}";
	}

    private void OptionsButton_Clicked(object sender, EventArgs e)
    {
        OptionsInterface.IsVisible = !OptionsInterface.IsVisible;
        double currRot = OptionsButton.Rotation;
        OptionsButton.RotateTo(currRot == 0 ? 45 : 0);
    }

    private void ReplicateButton_Clicked(object sender, EventArgs e)
    {

    }

    private void SolveButton_Clicked(object sender, EventArgs e)
    {

    }

    private void RemoveButton_Clicked(object sender, EventArgs e)
    {

    }

    private void EditButton_Clicked(object sender, EventArgs e)
    {

    }
}