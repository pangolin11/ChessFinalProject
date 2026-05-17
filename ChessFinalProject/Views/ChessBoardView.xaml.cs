using ChessFinalProject.Service.DBService.Firebase;
using ChessFinalProject.ViewModels;

namespace ChessFinalProject.Views;

public partial class ChessBoardView : ContentPage
{
	public ChessBoardView(ChessBoardViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ChessBoardViewModel vm)
        {
            // only initialize once
            if (vm.Board.Count == 0)
            {
                await vm.StartGame();

            }
        }
    }

    protected override async void OnDisappearing()
    {
        if (BindingContext is ChessBoardViewModel vm)
        {
            
            await vm.EndGame();
        }
    }
}