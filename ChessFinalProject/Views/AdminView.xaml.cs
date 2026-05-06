using ChessFinalProject.ViewModels;

namespace ChessFinalProject.Views;

public partial class AdminView : ContentPage
{
	public AdminView(AdminViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}