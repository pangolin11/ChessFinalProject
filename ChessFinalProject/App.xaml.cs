using Microsoft.Extensions.DependencyInjection;
using ChessFinalProject.Views;

namespace ChessFinalProject
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new ChessBoardViewxaml());
        }
    }
}