using RobotManager.Classes;

namespace RobotManager.Views;

public partial class AddClinicVisitPage : ContentPage
{
	Nao _nao;
	ClinicVisit _clinicVisit;
	List<int> issues = new();
	public AddClinicVisitPage(Nao nao, ClinicVisit clinicVisit, bool creatorMode = false)
	{
		InitializeComponent();
		BindingContext = nao;
		_nao = nao;
        _clinicVisit = clinicVisit;
    }

    private void CreateVisit_Button_Clicked(object sender, EventArgs e)
    {
		//_nao.ClinicVisits.Add(_clinicVisit.Id);

		_nao.Status = Status.Clinic;
    }

}