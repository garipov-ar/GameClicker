using Microsoft.Maui.Controls;

namespace Game.Pages
{
    public partial class GameOverPage : ContentPage
    {
        public GameOverPage()
        {
            InitializeComponent();
        }

        private async void OnRestartButtonClicked(object sender, EventArgs e)
        {
            // Возвращаемся на главную страницу
            await Navigation.PopAsync();
        }
    }
}