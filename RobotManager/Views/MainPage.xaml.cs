namespace RobotManager.Views;
using static Classes.Helpers;
using Classes;

public partial class MainPage : ContentPage
{

    private List<ClinicVisit>? clinicVisits = new();

    public MainPage()
    {
        InitializeComponent();
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        clinicVisits = await GetAllClinicVisits();
        ClinicVisitCV.ItemsSource = clinicVisits;
    }

    

    private void ClinicVisitFrame_Tapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        if (frame == null) { return; }
        var visit = frame.BindingContext as ClinicVisit;
        if (visit == null) { return; }
        Navigation.PushAsync(new ClinicVisitDetailPage(visit));
    }
}


