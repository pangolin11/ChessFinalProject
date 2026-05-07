using ChessFinalProject.ViewModels;

namespace ChessFinalProject.Views;

public partial class ChessBoardView : ContentPage
{
	public ChessBoardView()
	{
		InitializeComponent();
		BindingContext = new ViewModels.ChessBoardViewModel();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ChessBoardViewModel vm)
        {
            // only initialize once
            if (vm.Board.Count == 0)
                await vm.InitializeBoardAsync(batchSize: 8, delayMs: 16);
        }
    }
}