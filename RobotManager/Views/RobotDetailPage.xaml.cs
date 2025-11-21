using RobotManager.Classes;
using static RobotManager.Classes.Helpers;
namespace RobotManager.Views;

public partial class RobotDetailPage : ContentPage
{
    private Robot robot;
    private Grid? currenDetailButtons;

    List<Issue>? issues = new();
    List<Note>? notes = new();
    List<ClinicVisit>? clinicVisits = new();

    Button selectedTab;

    public RobotDetailPage(int robotId)
    {
        InitializeComponent();
        _ = InitializeAsync(robotId);
    }

    private async Task InitializeAsync(int robotId)
    {
        robot = new();

        if (!await robot.InitializeFromCloud(robotId)) { return; }
        issues = await GetGroupIssues(robotId);
        notes = await GetGroupNotes(robotId);
        clinicVisits = await GetGroupClinicVisits(robotId);
        if (notes != null)
        {
            foreach (var note in notes)
            {
                await note.AuthorUser.GetUser(note.Author);
            }
        }

        // check whether the robot is under warranty and display the corresponding text
        //bool underWarranty = robot.Warranty >= DateTime.Now;
        //warrantyLabel.Text = underWarranty ? $"Currently under warranty ({robot.Warranty.ToString("dd.MM.yyyy")})" : "Not under warranty";

        BindingContext = robot;
        NoteCV.ItemsSource = notes;

        // almost equivalent to 'NoteCV.ItemsSource = notes;' but filters out solved issues and clinic visits
        HandleSolvedIssues(solvedIssuesSwitch.IsToggled);
        HandlePastVisits(pastVisitSwitch.IsToggled);
    }

    // AddButton = '+' button in the bottom right corner
    private void AddButton_Clicked(object sender, EventArgs e)
    {
        // toggle visibility of the add button interface
        if (AddInterface.IsVisible || OptionsInterface.IsVisible)
        {
            AddInterface.IsVisible = false;
            OptionsInterface.IsVisible = false;
            DeleteEditButton.IsVisible = false;

            //transforms the 'x' button to an '+' button
            AddButton.RotateTo(0);
        }
        else
        {
            AddInterface.IsVisible = true;
            DeleteEditButton.IsVisible = true;

            // cancel out currently active detail buttons
            if (currenDetailButtons != null)
            {
                currenDetailButtons.IsVisible = false;
                currenDetailButtons = null;
            }

            //transforms the '+' button to an 'x' button
            AddButton.RotateTo(45);
        }
    }

    // habdles cases for buttons in the AddInterface
    private void AddNavigationButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        ContentPage page = new ContentPage();

        if (button == AddIssueButton)
        {
            page = new AddIssuePage(robot);
        }
        else if (button == AddNoteButton)
        {
            page = new AddNotePage(robot);
        }
        else if (button == AddClinicVisitButton)
        {
            page = new AddClinicVisitPage(robot, issues);
        }

        Navigation.PushAsync(page);
    }

    private void DeleteEditButton_Clicked(object sender, EventArgs e)
    {
        if (OptionsInterface.IsVisible)
        {
            OptionsInterface.IsVisible = false;
            AddInterface.IsVisible = true;
            DeleteEditButton.Text = "Delete/Edit";
        }
        else
        {
            OptionsInterface.IsVisible = true;
            AddInterface.IsVisible = false;
            DeleteEditButton.Text = "Add to NAO";
        }
    }

    private async void RemoveButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Delete", "Are you sure you want to delete this NAO?", "Yes", "No");
        if (!answer) { return; }
        //if (!await robot.Delete())
        //{
        //    await DisplayAlert("Error", "NAO could not be deleted", "OK");
        //}
    }

    private async void EditButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddRobotPage(robot, true));
    }

    // toggles the visibility of the detail buttons for notes issues and clinic visits
    private void Frame_Tapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        if (frame == null) { return; }
        var layout = frame.Content as VerticalStackLayout;
        var detailButtons = layout.FindByName<Grid>("DetailButtons");

        if (!detailButtons.IsVisible)
        {
            if (currenDetailButtons != null)
            {
                currenDetailButtons.IsVisible = false;
            }
            detailButtons.IsVisible = true;
            currenDetailButtons = detailButtons;
        }
        else
        {
            detailButtons.IsVisible = false;
            currenDetailButtons = null;
        }

        // cancel out currently active add interface
        if (AddInterface.IsVisible || OptionsInterface.IsVisible)
        {
            AddInterface.IsVisible = false;
            OptionsInterface.IsVisible = false;
            DeleteEditButton.IsVisible = false;

            //transforms the 'x' button to an '+' button
            AddButton.RotateTo(0);
        }
    }

    // note detail buttons
    private void ViewEditNoteButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) { return; }
        var note = button.BindingContext as Note;
        if (note == null) { return; }
        Navigation.PushAsync(new NoteDetailPage(note));
    }

    private async void RemoveNoteButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) { return; }
        var note = button.BindingContext as Note;
        if (note == null) { return; }

        bool answer = await DisplayAlert("Delete", "Are you sure you want to delete this note?", "Yes", "No");
        if (!answer) { return; }
        if (!await note.Delete())
        {
            // TODO: update list

            await DisplayAlert("Error", "Note could not be deleted", "OK");
        }
    }

    // issue detail buttons
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
        if (solvedReport == null)
        {
            // User pressed Cancel
            // use string.IsNullOrWhiteSpace(solvedReport) to check if user pressed OK with empty input
            return;
        }
        issue.Solved = (Preferences.Get("userID", -1), DateTime.Now);
        issue.SolvedReport = solvedReport;
        if (!await issue.SolveIssue())
        {
            await DisplayAlert("Error", "Issue could not be updated", "OK");
        }
    }

    // clinic visit detail buttons
    private void ViewEditClinicButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) { return; }
        var visit = button.BindingContext as ClinicVisit;
        if (visit == null) { return; }
        Navigation.PushAsync(new ClinicVisitDetailPage(visit));
    }

    private async void ReturnedClinicButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) { return; }
        var visit = button.BindingContext as ClinicVisit;
        if (visit == null) { return; }

        string backReport = await DisplayPromptAsync("Return Report", "Please enter a report:");
        if (backReport == null)
        {
            // User pressed Cancel
            // use string.IsNullOrWhiteSpace(backReport) to check if user pressed OK with empty input
            return;
        }

        bool issueSolved = await DisplayAlert("Issue(s) Solved?", "Mark issue(s) solved?", "Yes", "No");
        visit.BackDate = DateTime.Now;
        visit.BackReport = backReport;
        if (issueSolved && issues != null)
        {
            foreach (var issue in issues)
            {
                issue.Solved = (-2, DateTime.Now); // -2 > clinic
                issue.SolvedReport = $"Issue solved during clinic visit #{visit.Id.ToString().PadLeft(4, '0')}";
                if (!await issue.Post())
                {
                    await DisplayAlert("Error", "Issue could not be updated", "OK");
                }
            }
        }

        if (!await visit.EndClinicVisit())
        {
            await DisplayAlert("Error", "Clinic Visit could not be updated", "OK");
        }
    }

    // toggles between the three tabs
    private void TabButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        selectedTab = button!;

        UpdateTabs();
    }

    private void UpdateTabs()
    {
        Color normal = Color.FromArgb("#303030");
        Dictionary<Button, (Color color, StackLayout tab)> config = new()
        {
            { NotesButton, (Color.FromArgb("#308a7b"), NoteLayout ) },
            { IssuesButton, (Color.FromArgb("#80464d"), IssueLayout ) },
            { ClinicButton, (Color.FromArgb("#80b2c9"), ClinicLayout ) }
        };

        foreach (var button in config.Keys)
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
            IssueCV.ItemsSource = issues;
        }
        else
        {
            IssueCV.ItemsSource = issues.Where(issue => !issue.IsSolved).ToList();

        }

    }

    private void HandlePastVisits(bool toggled)
    {
        ClinicCV.ItemsSource = null;
        if (toggled)
        {
            ClinicCV.ItemsSource = clinicVisits;
        }
        else
        {
            ClinicCV.ItemsSource = clinicVisits.Where(visit => !visit.IsBack).ToList();

        }

    }

    private async void RobotDetailPageRV_Refreshing(object sender, EventArgs e)
    {
        await InitializeAsync(robot.Id);
        RobotDetailPageRV.IsRefreshing = false;
    }
}