using RobotManager.Classes;

namespace RobotManager.Views;

public partial class ClinicVisitPage : ContentPage
{
	Nao _nao;
	ClinicVisit clinicVisit = new();
	public ClinicVisitPage(Nao nao)
	{
		InitializeComponent();
		BindingContext = nao;
		_nao = nao;
	}

    private void CreateVisit_Button_Clicked(object sender, EventArgs e)
    {
		_nao.ClinicVisits.Add(clinicVisit);
		_nao.Status = Status.Clinic;
    }
}