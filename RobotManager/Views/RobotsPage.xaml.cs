using RobotManager.Classes;
using System.Collections.ObjectModel;

namespace RobotManager.Views;

public partial class RobotsPage : ContentPage
{
    ObservableCollection<Nao> naos = new();
    SwipeView? openSwipeView;

    public RobotsPage()
    {
        InitializeComponent();
        naos.Add(new() { Name = "26", BodyID = "28", HeadID = "20", Purchased = new DateTime(2023, 12, 18), Status = Status.Free });
        naos.Add(new() { Name = "25", BodyID = "06", HeadID = "06", Purchased = new DateTime(2023, 12, 18), Status = Status.Free });
        naos.Add(new() { Name = "24", BodyID = "11", HeadID = "32", Purchased = new DateTime(2022, 10, 21), Status = Status.Free });
        naos.Add(new() { Name = "23", BodyID = "10", HeadID = "37", Purchased = new DateTime(2022, 10, 21), Status = Status.Free });
        naos.Add(new() { Name = "22", BodyID = "04", HeadID = "22", Purchased = new DateTime(2022, 10, 21), Status = Status.Free });
        naos.Add(new() { Name = "21", BodyID = "42", HeadID = "50", Purchased = new DateTime(2022, 10, 21), Status = Status.Free });
        naos.Add(new() { Name = "18", BodyID = "29", HeadID = "37", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        naos.Add(new() { Name = "17", BodyID = "04", HeadID = "09", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        naos.Add(new() { Name = "16", BodyID = "17", HeadID = "31", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        naos.Add(new() { Name = "15", BodyID = "06", HeadID = "17", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        naos.Add(new() { Name = "14", BodyID = "40", HeadID = "45", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        naos.Add(new() { Name = "13", BodyID = "38", HeadID = "20", Purchased = new DateTime(2019, 12, 02), Status = Status.Free });
        naos.Add(new() { Name = "12", BodyID = "01", HeadID = "31", Purchased = new DateTime(2018, 05, 15), Status = Status.Free });
        naos.Add(new() { Name = "11", BodyID = "11", HeadID = "06", Purchased = new DateTime(2018, 05, 15), Status = Status.Free });
        RobotCollection.ItemsSource = naos;
    }

    private void SwipeItem_Clicked(object sender, EventArgs e)
    {
        var swipeItem = sender as SwipeItem;
        if (swipeItem == null) return;
        var nao = swipeItem.BindingContext as Nao;
        if (nao == null) return;

        switch (swipeItem.Text)
        {
            case "Free":
                nao.Status = Status.Free;
                break;
            case "Game":
                nao.Status = Status.Game;
                break;
            case "Clinic":
                Navigation.PushAsync(new ClinicVisitPage(nao));
                break;
            default:
                nao.Status = Status.Free;
                break;

        }


    }

    private void Robot_Tapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        if (frame == null) return;
        var nao = frame.BindingContext as Nao;
        if (nao == null) return;

        if (openSwipeView == null)
        {
            Navigation.PushAsync(new RobotDetailPage(nao));
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
        openSwipeView = swipeView;
    }
}