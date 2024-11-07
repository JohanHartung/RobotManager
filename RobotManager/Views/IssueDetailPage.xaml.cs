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