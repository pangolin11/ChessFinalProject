namespace ChessFinalProject.Views;

public partial class ChessBoardViewxaml : ContentPage
{
	public ChessBoardViewxaml()
	{
		InitializeComponent();
		BindingContext = new ViewModels.ChessBoardViewModel();
    }
}