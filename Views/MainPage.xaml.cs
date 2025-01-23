using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using Game.ViewModels;

namespace Game.Views
{
    public partial class MainPage : ContentPage
    {
        public MainViewModel ViewModel { get; set; }

        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            BindingContext = ViewModel;

            ViewModel.GameOverRequested += OnGameOverRequested;
            ViewModel.ShowAnimationRequested += OnShowAnimationRequested;
            ViewModel.ButtonAnimationRequested += OnButtonAnimationRequested;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel.GameOverRequested -= OnGameOverRequested;
            ViewModel.ShowAnimationRequested -= OnShowAnimationRequested;
            ViewModel.ButtonAnimationRequested -= OnButtonAnimationRequested;
        }

        private async void OnGameOverRequested()
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Navigation.PushAsync(new GameOverPage());
            });
        }

        private async void OnButtonAnimationRequested(string animationType)
        {
            if (animationType == "BlockButtonClicked")
            {
                var button = FindByName("BlockButton") as ImageButton;
                if (button != null)
                {
                    await button.ScaleTo(0.8, 100); // Уменьшение размера кнопки
                    await button.ScaleTo(1.0, 100); // Возврат к исходному размеру
                }
            }
        }

        private async void OnShowAnimationRequested(string message)
        {
            Color textColor = message.Contains("куплен") || message.Contains("создано") ? Colors.Green : Colors.Red;
            await ShowPurchaseAnimation(message, textColor);
        }

        private async Task ShowPurchaseAnimation(string message, Color textColor)
        {
            if (GameArea == null) return;

            var label = new Label
            {
                Text = message,
                TextColor = textColor,
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Opacity = 0,
                Scale = 0.5
            };

            GameArea.Children.Add(label);

            await Task.WhenAll(
                label.FadeTo(1, 200),
                label.ScaleTo(1, 200)
            );

            await label.TranslateTo(0, -50, 500, Easing.SinOut);

            await Task.WhenAll(
                label.FadeTo(0, 200),
                label.ScaleTo(0.5, 200)
            );

            GameArea.Children.Remove(label);
        }
    }
}