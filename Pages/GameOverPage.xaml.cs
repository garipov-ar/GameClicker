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
            // ������������ �� ������� ��������
            await Navigation.PopAsync();
        }
    }
}