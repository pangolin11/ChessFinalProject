using ChessFinalProject.ViewModels;
using ChessFinalProject.Views;

namespace ChessFinalProject
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel vm)
        {
            InitializeComponent();
			BindingContext = vm;
			Routing.RegisterRoute(nameof(MainPageView), typeof(MainPageView));
			Routing.RegisterRoute(nameof(AdminView), typeof(AdminView));
			Routing.RegisterRoute(nameof(AccountView), typeof(AccountView));
			Routing.RegisterRoute(nameof(UsersListView), typeof(UsersListView));

		}
	}
}
