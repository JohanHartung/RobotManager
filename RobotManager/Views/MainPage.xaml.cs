using RobotManager.Classes;
using System.Timers;

namespace RobotManager.Views;

public partial class MainPage : ContentPage
{
    public System.Timers.Timer timer;
    List<Game> MockGames;
    public MainPage()
    {
        InitializeComponent();


        MockGames = new()
        {
            new Game
            {
                Id = 1,
                Date = DateTime.Now.AddMinutes(45),
                Against = "B-Human",
                Field = Field.A,
                Home = true
            },
            new Game
            {
                Id = 2,
                Date = DateTime.Now.AddMinutes(500),
                Against = "Nao-Team HTWK",
                Field = Field.B,
                Home = false
            },
            new Game
            {
                Id = 3,
                Date = new DateTime(2024, 5, 22, 16, 45, 0),
                Against = "UT Austin Villa",
                Field = Field.C,
                Home = true
            },
            new Game
            {
                Id = 4,
                Date = new DateTime(2024, 6, 30, 13, 15, 0),
                Against = "SPQR Team",
                Field = Field.D,
                Home = false
            },
            new Game
            {
                Id = 5,
                Date = new DateTime(2024, 7, 12, 18, 0, 0),
                Against = "NomadZ",
                Field = Field.A,
                Home = true
            }
        };
        GamesCV.ItemsSource = MockGames;


        StartCountdown();
    }

    private void StartCountdown()
    {
        timer = new System.Timers.Timer(1000); 
        timer.Elapsed += TimerElapsed;
        timer.Start();
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        foreach (var game in MockGames)
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
        Dispatcher.Dispatch(() => GamesCV.ItemsSource = MockGames);
    }


}


