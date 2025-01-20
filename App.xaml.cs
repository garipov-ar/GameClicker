﻿using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Game
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            // Устанавливаем размеры окна (ширина и высота в пикселях)
            window.Width = 700;
            window.Height = 900;

            // Опционально: задаем минимальные и максимальные размеры окна
            window.MinimumWidth = 700;
            window.MinimumHeight = 900;
            window.MaximumWidth = 700;
            window.MaximumHeight = 900;

            // Опционально: задаем начальную позицию окна
            window.X = 100;
            window.Y = 100;

            return window;
        }
    }
}