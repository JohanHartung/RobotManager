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
        NoteCV.ItemsSource = _notes;
        HandleSolvedIssues(solvedIssuesSwitch.IsToggled);
        HandlePastVisits(pastVisitSwitch.IsToggled);
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
        var detailButtons = layout.FindByName<Grid>("DetailButtons");

        if (!detailButtons.IsVisible)
        {
            if (currentIssueButtons != null)
            {
                currentIssueButtons.IsVisible = false;
            }
            detailButtons.IsVisible = true;
            currentIssueButtons = detailButtons;
        }
        else
        {
            detailButtons.IsVisible = false;
            currentIssueButtons = null;
        }
    }

    private void ViewEditNoteButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) { return; }
        var note = button.BindingContext as Note;
        if (note == null) { return; }
        Navigation.PushAsync(new NoteDetailPage(note, _nao));
    }

    private void ViewEditIssueButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) { return; }
        var issue = button.BindingContext as Issue;
        if (issue == null) { return; }
        Navigation.PushAsync(new IssueDetailPage(issue, _nao));
    }
    private void ViewEditClinicButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) { return; }
        var visit = button.BindingContext as ClinicVisit;
        if (visit == null) { return; }
        Navigation.PushAsync(new ClinicVisitDetailPage(visit, _nao, _issues));
    }

    private async void SolvedIssueButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) { return; }
        var issue = button.BindingContext as Issue;
        if (issue == null) { return; }

        string solvedReport = await DisplayPromptAsync("Solved Report", "If issue could not be replicated, leave empty");
        issue.Solved = true;
        issue.SolvedReport = solvedReport;
        if (!await issue.Post())
        {
            await DisplayAlert("Error", "Issue could not be updated", "OK");
        }
    }
    private async void ReturnedClinicButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) { return; }
        var visit = button.BindingContext as ClinicVisit;
        if (visit == null) { return; }

        string backReport = await DisplayPromptAsync("Return Report", "Please enter a report:");
        bool issueSolved = await DisplayAlert("Issue(s) Solved?", "Mark issue(s) solved?", "Yes", "No");
        visit.IsBack = true;
        visit.BackDate = DateTime.Now;
        visit.BackReport = backReport;
        if (issueSolved)
        {
            foreach (var issue in _issues)
            {
                issue.Solved = true;
                issue.SolvedDate = DateTime.Now;
                issue.SolvedReport = $"Issue solved during clinic visit #{visit.Id.ToString().PadLeft(4, '0')}";
                if (!await issue.Post())
                {
                    await DisplayAlert("Error", "Issue could not be updated", "OK");
                }
            }
        }

        if (!await visit.Post())
        {
            await DisplayAlert("Error", "Clinic Visit could not be updated", "OK");
        }
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

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        var toggleSwitch = sender as Switch;
        if (toggleSwitch == solvedIssuesSwitch)
        {
            HandleSolvedIssues(toggleSwitch.IsToggled);
        }
        else if (toggleSwitch == pastVisitSwitch)
        {
            HandlePastVisits(toggleSwitch.IsToggled);
        }
    }


    private void SwitchGrid_Tapped(object sender, TappedEventArgs e)
    {
        var grid = sender as Grid;
        var toggleSwitch = new Switch();
        if (grid == solvedIssuesGrid)
        {
            toggleSwitch = grid!.FindByName<Switch>("solvedIssuesSwitch");
            HandleSolvedIssues(!toggleSwitch.IsToggled);
        }
        else if (grid == pastVisitGrid)
        {
            toggleSwitch = grid!.FindByName<Switch>("pastVisitSwitch");
            HandlePastVisits(!toggleSwitch.IsToggled);
        }

        toggleSwitch.IsToggled = !toggleSwitch.IsToggled;
    }

    private void HandleSolvedIssues(bool toggled)
    {
        IssueCV.ItemsSource = null;
        if (toggled)
        {
            IssueCV.ItemsSource = _issues;
        }
        else
        {
            IssueCV.ItemsSource = _issues.Where(issue => !issue.Solved).ToList();
            
        }

    }

    private void HandlePastVisits(bool toggled)
    {
        ClinicCV.ItemsSource = null;
        if (toggled)
        {
            ClinicCV.ItemsSource = _clinicVisits;
        }
        else
        {
            ClinicCV.ItemsSource = _clinicVisits.Where(visit => !visit.IsBack).ToList();
            
        }

    }

}