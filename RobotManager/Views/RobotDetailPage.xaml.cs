using RobotManager.Classes;

namespace RobotManager.Views;

public partial class RobotDetailPage : ContentPage
{
	public RobotDetailPage(Nao nao)
	{
		InitializeComponent();

		BindingContext = nao;
	}
}