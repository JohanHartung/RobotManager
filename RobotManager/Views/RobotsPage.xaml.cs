using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using RobotManager.Classes;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace RobotManager.Views;

public partial class RobotsPage : ContentPage
{
    ObservableCollection<Nao> naos = new();
    List<Issue> issues = new();
    List<Note> notes = new();
    List<ClinicVisit> clinicVisits = new();
    SwipeView? openSwipeView;

    Button selectedFilter;

    public RobotsPage()
    {
        InitializeComponent();
        selectedFilter = AllFilterButton;

#if DEBUG
        issues = new List<Issue>
{
    new()
    {
        Id = 1,
        Title = "Joint Motor Calibration Error",
        Date = new DateTime(2023, 12, 18),
        Description = "The right elbow joint is misaligned and requires recalibration.",
        Replicated = true,
        ReplicatedDate = new DateTime(2023, 12, 18),
        Solved = false,
        Nao = 3
    },
    new()
    {
        Id = 2,
        Title = "Speech Recognition Module Crash",
        Date = new DateTime(2024, 1, 5),
        Description = "The NAO robot fails to recognize simple commands after multiple interactions.",
        Replicated = false,
        Solved = true,
        Nao = 1
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
        Nao = 2
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
        Nao = 4
    },
    new()
    {
        Id = 5,
        Title = "Movement Freeze During Interaction",
        Date = new DateTime(2024, 4, 7),
        Description = "The robot freezes while performing dance routines, requiring a reboot.",
        Replicated = false,
        Solved = false,
        Nao = 2
    }
};
        notes = new List<Note>
{
    new()
    {
        Id = 1,
        Nao = 3,
        Title = "Initial Calibration Note",
        Description = "Calibrated joints and sensors for optimal performance. Right elbow still requires adjustment.",
        Date = new DateTime(2023, 12, 18)
    },
    new()
    {
        Id = 2,
        Nao = 1,
        Title = "Speech Recognition Issue",
        Description = "Noticed intermittent failures in the speech recognition module. Robot struggles with noise-heavy environments.",
        Date = new DateTime(2024, 1, 6)
    },
    new()
    {
        Id = 3,
        Nao = 2,
        Title = "Battery Replacement",
        Description = "Replaced the battery due to rapid drain issues. Testing results to be monitored over the next week.",
        Date = new DateTime(2024, 2, 12)
    },
    new()
    {
        Id = 4,
        Nao = 4,
        Title = "Camera Feed Issue",
        Description = "Camera displays distorted images in low light. Firmware update recommended.",
        Date = new DateTime(2024, 3, 23)
    },
    new()
    {
        Id = 5,
        Nao = 2,
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
        Nao = 3,
        Date = new DateTime(2023, 12, 18),
        Issues = new List<int> { 1, 3 },
        IsBack = false,
        BackReport = String.Empty
    },
    new()
    {
        Id = 2,
        Nao = 1,
        Date = new DateTime(2024, 1, 5),
        Issues = new List<int> { 2 },
        IsBack = true,
        BackReport = "Speech recognition issue fixed with module replacement."
    },
    new()
    {
        Id = 3,
        Nao = 2,
        Date = new DateTime(2024, 2, 15),
        Issues = new List<int> { 3, 5 },
        IsBack = false,
        BackReport = String.Empty
    },
    new()
    {
        Id = 4,
        Nao = 4,
        Date = new DateTime(2024, 3, 23),
        Issues = new List<int> { 4 },
        IsBack = true,
        BackReport = "Firmware update resolved camera distortion issue."
    },
    new()
    {
        Id = 5,
        Nao = 2,
        Date = new DateTime(2024, 4, 10),
        Issues = new List<int> { 5 },
        IsBack = false,
        BackReport = String.Empty
    }
};


        naos.Add(new() { Id = 1, Name = "26", BodyID = "28", HeadID = "20", Purchased = new DateTime(2023, 12, 18), Status = Status.Free });
        naos.Add(new() { Id = 2, Name = "25", BodyID = "06", HeadID = "06", Purchased = new DateTime(2023, 12, 18), Status = Status.Free });
        naos.Add(new() { Id = 3, Name = "24", BodyID = "11", HeadID = "32", Purchased = new DateTime(2022, 10, 21), Status = Status.Free });
        naos.Add(new() { Id = 4, Name = "23", BodyID = "10", HeadID = "37", Purchased = new DateTime(2022, 10, 21), Status = Status.Free });
        naos.Add(new() { Id = 5, Name = "22", BodyID = "04", HeadID = "22", Purchased = new DateTime(2022, 10, 21), Status = Status.Free });
        naos.Add(new() { Id = 6, Name = "21", BodyID = "42", HeadID = "50", Purchased = new DateTime(2022, 10, 21), Status = Status.Free });
        naos.Add(new() { Id = 7, Name = "18", BodyID = "29", HeadID = "37", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        naos.Add(new() { Id = 8, Name = "17", BodyID = "04", HeadID = "09", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        naos.Add(new() { Id = 9, Name = "16", BodyID = "17", HeadID = "31", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        naos.Add(new() { Id = 10, Name = "15", BodyID = "06", HeadID = "17", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        naos.Add(new() { Id = 11, Name = "14", BodyID = "40", HeadID = "45", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        naos.Add(new() { Id = 12, Name = "13", BodyID = "38", HeadID = "20", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        naos.Add(new() { Id = 13, Name = "12", BodyID = "01", HeadID = "31", Purchased = new DateTime(2018, 05, 15), Status = Status.Free });
        naos.Add(new() { Id = 14, Name = "11", BodyID = "11", HeadID = "06", Purchased = new DateTime(2018, 05, 15), Status = Status.Free });
        RobotCollection.ItemsSource = naos;
#endif
    }

    private async Task LoadNaosAsync()
    {
        try
        {
            using HttpClient client = new();
            var response = await client.GetAsync("https://skakominor.de/api/RobotManager/GetAll/naos");
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonResponse);

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<Nao>>(jsonResponse);
            if (apiResponse?.Value != null)
            {
                foreach (var nao in apiResponse.Value)
                {
                    if (!naos.Contains(nao))
                    {
                        naos.Add(nao);
                    }
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
        var nao = swipeItem.BindingContext as Nao;
        if (nao == null) return;

        if (openSwipeView != null)
        {
            openSwipeView.Close();
            openSwipeView = null;
        }

        switch (swipeItem.Text)
        {
            case "Free":
#if DEBUG
                nao.Status = Status.Free;
#endif
                await SetStatusAsync(nao, Status.Free);
                break;
            case "Game":
#if DEBUG
                nao.Status = Status.Game;
#endif
                await SetStatusAsync(nao, Status.Game);
                break;
            case "Clinic":
                await Navigation.PushAsync(new AddClinicVisitPage(nao, issues.Where(iss => iss.Nao == nao.Id).ToList()));
                break;
            default:
#if DEBUG
                nao.Status = Status.Free;
#endif
                await SetStatusAsync(nao, Status.Free);
                break;

        }
        FilterCollection();


    }

    private void Robot_Tapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        if (frame == null) return;
        var nao = frame.BindingContext as Nao;
        if (nao == null) return;

        if (openSwipeView == null)
        {
            var naoIssues = issues.Where(iss => iss.Nao == nao.Id).ToList();
            var naoNotes = notes.Where(note => note.Nao == nao.Id).ToList();
            var naoClinicVisits = clinicVisits.Where(clinic => clinic.Nao == nao.Id).ToList();
            Navigation.PushAsync(new RobotDetailPage(nao, naoIssues, naoNotes, naoClinicVisits));
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
        await LoadNaosAsync();
        RobotCollection.ItemsSource = null;
        FilterCollection();
        RobotPageRV.IsRefreshing = false;
    }

    private async Task SetStatusAsync(Nao nao, Status status)
    {
        using HttpClient client = new();
        string apiUrl = $"https://skakominor.de/api/RobotManager/SetStatus/{nao.Id}/{status}";
        try
        {
            HttpResponseMessage response = await client.PostAsync(apiUrl, null);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error syncing status", "OK");
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
            { AllFilterButton, Color.FromArgb("#455a64") },
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
            RobotCollection.ItemsSource = naos;
        }
        else
        {
            RobotCollection.ItemsSource = naos.Where(nao => nao.Status.ToString() == selectedFilter.Text);
        }
    }
}
