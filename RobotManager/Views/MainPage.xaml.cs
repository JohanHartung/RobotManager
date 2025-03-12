namespace RobotManager.Views;
using static Classes.Helpers;
using Classes;
using System.Timers;

public partial class MainPage : ContentPage
{

    private List<Game>? games = new();
    private List<ClinicVisit>? clinicVisits = new();
    public System.Timers.Timer timer;

    public MainPage()
    {
        InitializeComponent();
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        games = await GetAllGames();
        GamesCV.ItemsSource = games;
        clinicVisits = await GetAllClinicVisits();
        ClinicVisitCV.ItemsSource = clinicVisits;
        StartCountdown();
    }

    

    private void ClinicVisitFrame_Tapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        if (frame == null) { return; }
        var visit = frame.BindingContext as ClinicVisit;
        if (visit == null) { return; }
        Navigation.PushAsync(new ClinicVisitDetailPage(visit));
    }

    private async void RefreshView_Refreshing(object sender, EventArgs e)
    {
        await InitializeAsync();
        MainPageRV.IsRefreshing = false;
    }

    private void StartCountdown()
    {
        timer = new System.Timers.Timer(1000);
        timer.Elapsed += TimerElapsed;
        timer.Start();
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        foreach (var game in games)
        {
            var remaining = game.Date - DateTime.Now;

            if (remaining.TotalSeconds <= 0)
            {
                game.CountdownText = "Started!";
            }
            else if (remaining.TotalMinutes <= 60)
            {
                game.CountdownText = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            }
            else
            {
                game.CountdownText = null;
            }
        }

        // Refresh the UI on the main thread
        Dispatcher.Dispatch(() => GamesCV.ItemsSource = null);
        Dispatcher.Dispatch(() => GamesCV.ItemsSource = games);
    }

    private async void AddGameButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddGamePage());
    }
}


