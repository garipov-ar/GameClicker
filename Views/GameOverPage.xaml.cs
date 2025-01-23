using Microsoft.Maui.Controls;


namespace Game.Views
{
    public partial class GameOverPage : ContentPage
    {
        public GameOverPage()
        {
            InitializeComponent(); 
        }

        private async void OnRestartButtonClicked(object sender, EventArgs e)
        {

            if (sender is Button button)
            {
                await button.ScaleTo(0.8, 100);
                await button.ScaleTo(1.0, 100);
            }

            await Navigation.PopToRootAsync();
        }
    }
}