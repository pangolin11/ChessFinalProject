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
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var email = await SecureStorage.Default.GetAsync("UserEmail");
        var password = await SecureStorage.Default.GetAsync("UserPass");

        if (email == null || password == null)
            return;

        if (BindingContext is SignInViewModel vm)
        {
            vm.UserEmail = email;
            vm.UserPassword = password;
            vm.SignIn();
        }
        
    }
}