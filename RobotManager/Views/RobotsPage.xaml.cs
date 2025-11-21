using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using RobotManager.Classes;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Maui.Controls.PlatformConfiguration.TizenSpecific;


namespace RobotManager.Views;

public partial class RobotsPage : ContentPage
{
    private static readonly HttpClient client = new();
    ObservableCollection<Robot> robots = new();
    List<Issue> issues = new();
    List<Note> notes = new();
    List<ClinicVisit> clinicVisits = new();
    SwipeView? openSwipeView;

    Button selectedFilter;
    bool isOnline = Connectivity.NetworkAccess == NetworkAccess.Internet;

    public RobotsPage()
    {
        InitializeComponent();
        Refresh();

        selectedFilter = AllFilterButton;

#if false
        issues = new List<Issue>
{
    new()
    {
        Id = 1,
        Title = "Joint Motor Calibration Error",
        Date = new DateTime(2023, 12, 18),
        Description = "The right elbow joint is misaligned and requires recalibration.",
        Replicated = new Dictionary<int, DateTime> { { 1, new DateTime(2023, 12, 18) } },
        robot = 3
    }
};
        /*
    new()
    {
        Id = 2,
        Title = "Speech Recognition Module Crash",
        Date = new DateTime(2024, 1, 5),
        Description = "The NAO robot fails to recognize simple commands after multiple interactions.",
        Solved = (3, DateTime.Now),
        robot = 1
    },
    new()
    {
        Id = 3,
        Title = "Battery Drain Issue",
        Date = new DateTime(2024, 2, 10),
        Description = "The battery depletes rapidly even after a full charge, limiting operation time.",
        Replicated = true,
        ReplicatedDate = new DateTime(2024, 2, 15),
        Solved = false,
        robot = 2
    },
    new()
    {
        Id = 4,
        Title = "Camera Module Malfunction",
        Date = new DateTime(2024, 3, 22),
        Description = "The robot's camera feed displays distorted images under low-light conditions.",
        Replicated = true,
        ReplicatedDate = new DateTime(2024, 3, 23),
        Solved = true,
        robot = 4
    },
    new()
    {
        Id = 5,
        Title = "Movement Freeze During Interaction",
        Date = new DateTime(2024, 4, 7),
        Description = "The robot freezes while performing dance routines, requiring a reboot.",
        Replicated = false,
        Solved = true,
        robot = 2
    }
};*/
        notes = new List<Note>
{
    new()
    {
        Id = 1,
        robot = 3,
        Title = "Initial Calibration Note",
        Description = "Calibrated joints and sensors for optimal performance. Right elbow still requires adjustment.",
        Date = new DateTime(2023, 12, 18)
    },
    new()
    {
        Id = 2,
        robot = 1,
        Title = "Speech Recognition Issue",
        Description = "Noticed intermittent failures in the speech recognition module. Robot struggles with noise-heavy environments.",
        Date = new DateTime(2024, 1, 6)
    },
    new()
    {
        Id = 3,
        robot = 2,
        Title = "Battery Replacement",
        Description = "Replaced the battery due to rapid drain issues. Testing results to be monitored over the next week.",
        Date = new DateTime(2024, 2, 12)
    },
    new()
    {
        Id = 4,
        robot = 4,
        Title = "Camera Feed Issue",
        Description = "Camera displays distorted images in low light. Firmware update recommended.",
        Date = new DateTime(2024, 3, 23)
    },
    new()
    {
        Id = 5,
        robot = 2,
        Title = "Movement Freeze During Interaction",
        Description = "The robot froze while performing a routine. Suspected software bug.",
        Date = new DateTime(2024, 4, 8)
    }
};
        clinicVisits = new List<ClinicVisit>
{
    new()
    {
        Id = 1,
        robot = 3,
        Date = new DateTime(2023, 12, 18),
        Issues = new List<int> { 1, 3 },
        IsBack = false,
        Notes = "Camera displays distorted images in low light. Firmware update recommended.",
        BackReport = String.Empty
    },
    new()
    {
        Id = 2,
        robot = 1,
        Date = new DateTime(2024, 1, 5),
        Issues = new List<int> { 2 },
        IsBack = true,
        Notes = "Speech recognition module crash resolved with module replacement.",
        BackReport = "Speech recognition issue fixed with module replacement."
    },
    new()
    {
        Id = 3,
        robot = 2,
        Date = new DateTime(2024, 2, 15),
        Issues = new List<int> { 3, 5 },
        IsBack = false,
        Notes = "Battery replaced due to rapid drain issue. Testing results to be monitored.",
        BackReport = String.Empty
    },
    new()
    {
        Id = 4,
        robot = 4,
        Date = new DateTime(2024, 3, 23),
        Issues = new List<int> { 4 },
        IsBack = true,
        Notes = "Camera feed displays distorted images in low light. Firmware update recommended.",
        BackReport = "Firmware update resolved camera distortion issue."
    },
    new()
    {
        Id = 5,
        robot = 2,
        Date = new DateTime(2024, 4, 10),
        Issues = new List<int> { 5 },
        IsBack = false,
        Notes = "Robot froze during routine. Suspected software bug.",
        BackReport = String.Empty
    }
};


        robots.Add(new() { Id = 1, Name = "26", BodyID = "28", HeadID = "20", Purchased = new DateTime(2023, 12, 18), Status = Status.Free });
        robots.Add(new() { Id = 2, Name = "25", BodyID = "06", HeadID = "06", Purchased = new DateTime(2023, 12, 18), Status = Status.Free });
        robots.Add(new() { Id = 3, Name = "24", BodyID = "11", HeadID = "32", Purchased = new DateTime(2022, 10, 21), Status = Status.Free });
        robots.Add(new() { Id = 4, Name = "23", BodyID = "10", HeadID = "37", Purchased = new DateTime(2022, 10, 21), Status = Status.Free });
        robots.Add(new() { Id = 5, Name = "22", BodyID = "04", HeadID = "22", Purchased = new DateTime(2022, 10, 21), Status = Status.Free });
        robots.Add(new() { Id = 6, Name = "21", BodyID = "42", HeadID = "50", Purchased = new DateTime(2022, 10, 21), Status = Status.Free });
        robots.Add(new() { Id = 7, Name = "18", BodyID = "29", HeadID = "37", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        robots.Add(new() { Id = 8, Name = "17", BodyID = "04", HeadID = "09", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        robots.Add(new() { Id = 9, Name = "16", BodyID = "17", HeadID = "31", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        robots.Add(new() { Id = 10, Name = "15", BodyID = "06", HeadID = "17", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        robots.Add(new() { Id = 11, Name = "14", BodyID = "40", HeadID = "45", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        robots.Add(new() { Id = 12, Name = "13", BodyID = "38", HeadID = "20", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        robots.Add(new() { Id = 13, Name = "12", BodyID = "01", HeadID = "31", Purchased = new DateTime(2018, 05, 15), Status = Status.Free });
        robots.Add(new() { Id = 14, Name = "11", BodyID = "11", HeadID = "06", Purchased = new DateTime(2018, 05, 15), Status = Status.Free });
        RobotCollection.ItemsSource = robots;
#endif
    }

    private async Task LoadrobotsAsync()
    {
        try
        {
            //using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetAll/robots";
            var response = await client.GetAsync(apiUri);

            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            //check if "[]"
            if(jsonResponse == "[]")
            {
                await DisplayAlert("Error", "No robots found", "OK");
                return;
            }


            Console.WriteLine(jsonResponse);
            robots = new();
            var apiResponse = JsonSerializer.Deserialize<List<Robot>>(jsonResponse);
            if (apiResponse != null)
            {
                foreach (var robot in apiResponse)
                {
                    //if (!robots.Contains(robot))
                    //{
                    //}
                        robots.Add(robot);
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log the error, show a message to the user, etc.)
            await DisplayAlert("Error", $"Error fetching data: {ex.Message}", "OK");
            Console.WriteLine(ex);
        }
    }

    private async void SwipeItem_Clicked(object sender, EventArgs e)
    {
        var swipeItem = sender as SwipeItem;
        if (swipeItem == null) return;
        var robot = swipeItem.BindingContext as Robot;
        if (robot == null) return;

        if (openSwipeView != null)
        {
            openSwipeView.Close();
            openSwipeView = null;
        }

        switch (swipeItem.Text)
        {
            case "Free":


                if(!await robot.SetStatus(Status.Free))
                {
                    await DisplayAlert("Error", "Error syncing status", "OK");
                }
                break;
            case "Game":


                if (!await robot.SetStatus(Status.Game))
                {

                    await DisplayAlert("Error", "Error syncing status", "OK");
                }
                break;
            case "Clinic":
                await Navigation.PushAsync(new AddClinicVisitPage(robot, issues.Where(iss => iss.Robot == robot.Id).ToList()));
                break;
            default:


                if (!await robot.SetStatus(Status.Free))
                {
                    await DisplayAlert("Error", "Error syncing status", "OK");
                }
                break;

        }
        FilterCollection();


    }

    private void Robot_Tapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        if (frame == null) return;
        var robot = frame.BindingContext as Robot;
        if (robot == null) return;

        if (openSwipeView == null)
        {
            var robotIssues = issues.Where(iss => iss.Robot == robot.Id).ToList();
            var robotNotes = notes.Where(note => note.Robot == robot.Id).ToList();
            var robotClinicVisits = clinicVisits.Where(clinic => clinic.Robot == robot.Id).ToList();
            Navigation.PushAsync(new RobotDetailPage(robot.Id));
        }
        else
        {
            openSwipeView.Close();
            openSwipeView = null;
        }

    }

    private void SwipeView_SwipeStarted(object sender, SwipeStartedEventArgs e)
    {
        var swipeView = sender as SwipeView;
        if (swipeView == null) return;

        if (openSwipeView != null)
        {
            openSwipeView.Close();
            openSwipeView = null;
        }
    }

    private void SwipeView_SwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        var swipeView = sender as SwipeView;
        if (swipeView == null) return;
        openSwipeView = e.IsOpen ? swipeView : null;
    }

    private void SwipeView_SwipeChanging(object sender, SwipeChangingEventArgs e)
    {
        var swipeView = sender as SwipeView;
        if (swipeView == null) return;
    }

    private void AddButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AddRobotPage());
    }

    private async void RefreshView_Refreshing(object sender, EventArgs e)
    {
        await Refresh();
        RobotPageRV.IsRefreshing = false;
    }

    private async Task Refresh()
    {
        await LoadrobotsAsync();
        RobotCollection.ItemsSource = null;
        FilterCollection();
    }

    private async Task<bool> SetStatusAsync(Robot robot, Status status)
    {
        using HttpClient client = new();
        string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
        string apiUri = baseUri + $"SetStatus/{robot.Id}/{(int)status}";
        try
        {
            HttpResponseMessage response = await client.PostAsync(apiUri, new StringContent(""));
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error syncing status", "OK");
            return false;
        }
    }

    private void FilterButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;

        // set selectedFilter to AllFilter, if the same filter button is pressed twice
        selectedFilter = button == selectedFilter ? AllFilterButton! : button!;

        UpdateFilter();
        FilterCollection();
    }

    private void UpdateFilter()
    {

        Color normal = Color.FromArgb("#303030");

        Dictionary<Button, Color> config = new()
        {
            { AllFilterButton, Color.FromArgb("#536578") },
            { FreeFilterButton, Color.FromArgb("#308a7b") },
            { GameFilterButton, Color.FromArgb("#80464d") },
            { ClinicFilterButton, Color.FromArgb("#80b2c9") }
        };


        foreach (var button in config.Keys)
        {
            // set BackgroundColor for active  filter button
            button.BackgroundColor = button == selectedFilter ? config[button] : normal;
        }
    }


    private void FilterCollection()
    {
        if (selectedFilter == AllFilterButton)
        {
            RobotCollection.ItemsSource = robots;
        }
        else
        {
            RobotCollection.ItemsSource = robots.Where(robot => robot.Status.ToString() == selectedFilter.Text);
        }
    }
}
