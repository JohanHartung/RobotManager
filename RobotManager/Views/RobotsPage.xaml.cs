using RobotManager.Classes;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;


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
    Button selectedSubFilter;
    bool isOnline = Connectivity.NetworkAccess == NetworkAccess.Internet;

    public RobotsPage()
    {
        InitializeComponent();
        selectedFilter = AllFilterButton;
        selectedSubFilter = NaoAllFilterButton;
        Refresh();

    }

    private async Task LoadRobotsAsync()
    {
        try
        {
            //using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
            string apiUri = baseUri + $"robots/";
            Preferences.Set("apiKey", "c3e0702defe494b2b0bf4199ff5f77d3937f2a5d");
            string apiKey = Preferences.Get("apiKey", "");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", apiKey);

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
        await LoadRobotsAsync();
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
    private void SubFilterButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;

        // set selectedFilter to AllFilter, if the same filter button is pressed twice
        selectedSubFilter = button == selectedSubFilter ? NaoAllFilterButton! : button!;

        UpdateFilter();
        FilterCollection();
    }

    private void UpdateFilter()
    {

        Color normal = Color.FromArgb("#303030");

        Dictionary<Button, Color> config = new()
        {
            { AllFilterButton, Color.FromArgb("#536578") },
            { NaoFilterButton, Color.FromArgb("#308a7b") },
            { BoosterFilterButton, Color.FromArgb("#80464d") },
            { OtherFilterButton, Color.FromArgb("#80b2c9") },
            { NaoAllFilterButton, Color.FromArgb("#536578") },
            { NaoV5FilterButton, Color.FromArgb("#80464d") },
            { NaoV6FilterButton, Color.FromArgb("#80b2c9") }
        };


        foreach (var button in config.Keys)
        {
            // set BackgroundColor for active  filter button
            button.BackgroundColor = (button == selectedFilter || button == selectedSubFilter) ? config[button] : normal;
        }
    }


    private void FilterCollection()
    {
        var sortedRobots = robots.OrderBy(r => r.HeadNumber).ToList();
        robots = new ObservableCollection<Robot>(sortedRobots);

        NaoVersionFilterGrid.IsVisible = false;

        if (selectedFilter == AllFilterButton)
        {
            RobotCollection.ItemsSource = robots;
        }
        else if (selectedFilter == NaoFilterButton)
        {
            NaoVersionFilterGrid.IsVisible = true;
            var tempList = robots.Where(robot => robot.Model == "Nao");
            if (selectedSubFilter == NaoV5FilterButton)
            {
                tempList = tempList.Where(robot => robot.Version == "5.0 H25");
            }
            else if (selectedSubFilter == NaoV6FilterButton)
            {
                tempList = tempList.Where(robot => robot.Version == "6.0");
            }
            else
            {
                tempList = robots.Where(robot => robot.Model == "Nao");
            }
            RobotCollection.ItemsSource = tempList;
        }
        else if (selectedFilter == BoosterFilterButton)
        {
            RobotCollection.ItemsSource = robots.Where(robot => robot.Model.StartsWith("Booster"));
        }
        else
        {
            RobotCollection.ItemsSource = robots.Where(robot => robot.Model != "Nao" && !robot.Model.StartsWith("Booster"));
        }
    }
}
