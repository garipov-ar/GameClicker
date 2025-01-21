using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using Game.Pages;

namespace Game
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


            // Подписываемся на событие для анимаций
            ViewModel.ShowAnimationRequested += OnShowAnimationRequested;

            // Подписываемся на событие для анимации кнопки
            ViewModel.ButtonAnimationRequested += OnButtonAnimationRequested;
        }

        private async void OnGameOverRequested()
        {
            // Переходим на страницу GameOverPage
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Navigation.PushAsync(new GameOverPage());
            });
        }

        // Обработчик анимации кнопки
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

        // Обработчик анимации уведомлений
        private async void OnShowAnimationRequested(string message)
        {
            // Определяем цвет текста в зависимости от содержания сообщения
            Color textColor = message.Contains("куплен") || message.Contains("создано") ? Colors.Green : Colors.Red;
            await ShowPurchaseAnimation(message, textColor);
        }

        // Метод для отображения анимации покупки
        private async Task ShowPurchaseAnimation(string message, Color textColor)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                // Создаем Label для анимации
                var label = new Label
                {
                    Text = message,
                    TextColor = textColor,
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Opacity = 0, // Начальная прозрачность
                    Scale = 0.5  // Начальный масштаб
                };

                // Добавляем Label на экран
                if (GameArea != null)
                {
                    GameArea.Children.Add(label);

                    // Анимация всплывания
                    await Task.WhenAll(
                        label.FadeTo(1, 200), // Появление
                        label.ScaleTo(1, 200) // Увеличение
                    );

                    // Движение вверх
                    await label.TranslateTo(0, -50, 500, Easing.SinOut);

                    // Исчезновение
                    await Task.WhenAll(
                        label.FadeTo(0, 200), // Исчезновение
                        label.ScaleTo(0.5, 200) // Уменьшение
                    );

                    // Убираем Label с экрана
                    GameArea.Children.Remove(label);
                }
            });
        }
    }
}