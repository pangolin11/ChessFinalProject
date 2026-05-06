using ChessFinalProject.ViewModels;

namespace ChessFinalProject.Views;

public partial class MainPageView : ContentPage
{
	public MainPageView(MainPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}