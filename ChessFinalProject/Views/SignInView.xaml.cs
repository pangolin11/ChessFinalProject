using ChessFinalProject.ViewModels;

namespace ChessFinalProject.Views;

public partial class SignInView : ContentPage
{
	public SignInView(SignInViewModel vm)
	{
		InitializeComponent();
		vm.Navigation = this.Navigation;
		BindingContext = vm;
	}
}