namespace ChessFinalProject.Views;

public partial class ChessBoardView : ContentPage
{
	public ChessBoardView()
	{
		InitializeComponent();
		BindingContext = new ViewModels.ChessBoardViewModel();
    }
}