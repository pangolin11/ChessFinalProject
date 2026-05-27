using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Firebase.Database.Streaming;
using ChessFinalProject.Model;
using ChessFinalProject.Service;
using ChessFinalProject.Service.DBService;
using ChessFinalProject.Service.DBService.Firebase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessFinalProject.ViewModels
{
	public partial class MainPageViewModel: ObservableObject
	{
		[ObservableProperty]
		private string _name;
		public MainPageViewModel() 
		{
		}

		[RelayCommand]
		private async Task Settings()
		{
			await Shell.Current.GoToAsync("AccountView");
		}
		[RelayCommand]
		private async Task NavToChess()
		{
			await Shell.Current.GoToAsync("ChessBoardView");
        }

        internal void OnAppearing()
        {
            _name = "Hello " + (App.Current as App)!.CurrentUser!.FirstName!;
        }
    }
}
