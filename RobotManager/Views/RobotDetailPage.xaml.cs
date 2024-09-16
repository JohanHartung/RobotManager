using RobotManager.Classes;

namespace RobotManager.Views;

public partial class RobotDetailPage : ContentPage
{
    private Nao _nao;
    public RobotDetailPage(Nao nao)
    {
        InitializeComponent();

        BindingContext = nao;
        _nao = nao;

        bool underWarranty = nao.Warranty >= DateTime.Now;
        warrantyLabel.Text = underWarranty ? $"Currently under warranty ({nao.Warranty.ToString("dd.MM.yyyy")})" : "Not under warranty";
    }

    private void AddButton_Clicked(object sender, EventArgs e)
    {
        AddInterface.IsVisible = !AddInterface.IsVisible;
        //AddButton.Text = AddButton.Text == "+" ? "x" : "+";
        double currRot = AddButton.Rotation;
        AddButton.RotateTo(currRot == 0 ? 45 : 0);
    }

    private void AddNavigationButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        ContentPage page = new ContentPage();

        if (button == AddIssueButton)
        {
            page = new IssuePage(_nao, new(), true);
        }
        else if (button == AddNoteButton)
        {
            page = new NotePage(_nao, new(), true);
        }
        else if (button == AddClinicVisitButton)
        {
            page = new ClinicVisitPage(_nao, new(), true);
        }

        Navigation.PushAsync(page);
    }
}