using RobotManager.Classes;

namespace RobotManager.Views;

public partial class RobotDetailPage : ContentPage
{
    private Nao _nao;
    private Grid? currentIssueButtons;

    List<Issue> _issues = new();
    List<Note> _notes = new();
    List<ClinicVisit> _clinicVisits = new();

    Button selectedTab;

    public RobotDetailPage(Nao nao, List<Issue> issues, List<Note> notes, List<ClinicVisit> clinicVisits)
    {
        InitializeComponent();

        _nao = nao;
        _issues = issues;
        _notes = notes;
        _clinicVisits = clinicVisits;

        bool underWarranty = nao.Warranty >= DateTime.Now;
        warrantyLabel.Text = underWarranty ? $"Currently under warranty ({nao.Warranty.ToString("dd.MM.yyyy")})" : "Not under warranty";


        BindingContext = _nao;
        IssueCV.ItemsSource = _issues;
        NoteCV.ItemsSource = _notes;
        ClinicCV.ItemsSource = _clinicVisits;
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
            page = new AddIssuePage(_nao);
        }
        else if (button == AddNoteButton)
        {
            page = new AddNotePage(_nao);
        }
        else if (button == AddClinicVisitButton)
        {
            page = new AddClinicVisitPage(_nao, _issues);
        }

        Navigation.PushAsync(page);
    }

    private void Frame_Tapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        if (frame == null) { return; }
        var layout = frame.Content as VerticalStackLayout;
        var issueButtons = layout.FindByName<Grid>("DetailButtons");

        if (!issueButtons.IsVisible)
        {
            if (currentIssueButtons != null)
            {
                currentIssueButtons.IsVisible = false;
            }
            issueButtons.IsVisible = true;
            currentIssueButtons = issueButtons;
        }
        else
        {
            issueButtons.IsVisible = false;
            currentIssueButtons = null;
        }
    }

    private void ViewEditIssueButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) { return; }
        var issue = button.BindingContext as Issue;
        if (issue == null) { return; }
        Navigation.PushAsync(new IssueDetailPage(issue));
    }

    private async void SolvedIssueButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) { return; }
        var issue = button.BindingContext as Issue;
        if (issue == null) { return; }

        string solvedReport = await DisplayPromptAsync("Solved Report", "If issue could not be replicated, leave empty");
        //TODO: Update issue with solved report and date
    }

    private void TabButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        selectedTab = button!;

        UpdateTabs();
    }

    private void UpdateTabs()
    {
        Color normal = Color.FromArgb("#303030");
        Dictionary<Button, (Color color, StackLayout tab) > config = new()
        {
            { NotesButton, (Color.FromArgb("#308a7b"), NoteLayout ) },
            { IssuesButton, (Color.FromArgb("#80464d"), IssueLayout ) },
            { ClinicButton, (Color.FromArgb("#80b2c9"), ClinicLayout ) }
        };

        foreach ( var button in config.Keys ) 
        {
            if (button == selectedTab)
            {
                selectedTab.BackgroundColor = config[button].color;
                config[button].tab.IsVisible = true;
            }
            else
            {
                button.BackgroundColor = normal;
                config[button].tab.IsVisible = false;
            }
        }
    }

    private void PastIssuesGrid_Tapped(object sender, TappedEventArgs e)
    {
        var grid = sender as Grid;
        var issueSwitch = grid!.FindByName<Switch>("pastIssuesSwitch");
        issueSwitch.IsToggled = !issueSwitch.IsToggled;
    }

}