using ChessFinalProject.ViewModels;

namespace ChessFinalProject.Views;

public partial class AccountView : ContentPage
{
	public AccountView(AccountViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}