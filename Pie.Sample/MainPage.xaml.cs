using Pie;
namespace Pie.Sample;

public partial class MainPage : ContentPage
{
	bool teste;

	public MainPage()
	{
		InitializeComponent();
		this.BindingContext = new MainPageViewModel();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		//pgPie.ChangeMaxCircle(teste ? 360 :  183);
		pgPie.IsHalfCircle = !pgPie.IsHalfCircle;

    }
}