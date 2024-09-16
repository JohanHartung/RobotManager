using RobotManager.Classes;

namespace RobotManager.Views;

public partial class ClinicVisitPage : ContentPage
{
	Nao _nao;
	ClinicVisit _clinicVisit;
	public ClinicVisitPage(Nao nao, ClinicVisit clinicVisit, bool creatorMode = false)
	{
		InitializeComponent();
		BindingContext = nao;
		_nao = nao;
        _clinicVisit = clinicVisit;
    }

    private void CreateVisit_Button_Clicked(object sender, EventArgs e)
    {
		_nao.ClinicVisits.Add(_clinicVisit);
		_nao.Status = Status.Clinic;
    }
}