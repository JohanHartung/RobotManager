using RobotManager.Classes;

namespace RobotManager.Views;

public partial class IssueDetailPage : ContentPage
{
	public IssueDetailPage(Issue issue)
	{
		InitializeComponent();

		BindingContext = issue;
	}

    private void OptionsButton_Clicked(object sender, EventArgs e)
    {
        OptionsInterface.IsVisible = !OptionsInterface.IsVisible;
        //AddButton.Text = AddButton.Text == "+" ? "x" : "+";
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
}