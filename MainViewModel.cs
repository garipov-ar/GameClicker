using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Timer = System.Timers.Timer;
using Microsoft.Maui.ApplicationModel;
using System.Timers;

namespace Game
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _traffic = 0;
        private int _trafficPerClick = 1;
        private int _vpnCount = 0;
        private int _mirrorCount = 0;
        private double _progress;
        private ObservableCollection<Notification> _notifications = new ObservableCollection<Notification>();
        private const int TargetTraffic = 1000;
        private Timer _timer;
        private Timer _passiveIncomeTimer; // Таймер для пассивного дохода
        private Random _random = new Random();
        public event Action<string> ButtonAnimationRequested;
        private int _vpnPrice = 100;
        private int _mirrorPrice = 500;
        private int _level = 1;
        private int _levelTarget = 1000;

        public int VpnPrice
        {
            get => _vpnPrice;
            set
            {
                _vpnPrice = value;
                OnPropertyChanged();
            }
        }

        public int Level
        {
            get => _level;
            set
            {
                _level = value;
                OnPropertyChanged();
            }
        }

        public int LevelTarget
        {
            get => _levelTarget;
            set
            {
                _levelTarget = value;
                OnPropertyChanged();
            }
        }

        public int MirrorPrice
        {
            get => _mirrorPrice;
            set
            {
                _mirrorPrice = value;
                OnPropertyChanged();
            }
        }

        public int Traffic
        {
            get => _traffic;
            set
            {
                _traffic = value;
                OnPropertyChanged();
                UpdateProgress();
            }
        }

        public int TrafficPerClick
        {
            get => _trafficPerClick;
            set
            {
                _trafficPerClick = value;
                OnPropertyChanged();
            }
        }

        public int VpnCount
        {
            get => _vpnCount;
            set
            {
                _vpnCount = value;
                OnPropertyChanged();
            }
        }

        public int MirrorCount
        {
            get => _mirrorCount;
            set
            {
                if (_mirrorCount != value)
                {
                    _mirrorCount = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PassiveIncome)); // Уведомляем об изменении PassiveIncome
                }
            }
        }

        public class Notification
        {
            public string Message { get; set; } // Текст уведомления
            public bool IsPositive { get; set; } // true для позитивных, false для негативных
        }

        public int PassiveIncome => MirrorCount * 10; // Пассивный доход

        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                OnPropertyChanged();
            }
        }

        public ICommand BlockCommand { get; }
        public ICommand BuyVpnCommand { get; }
        public ICommand BuyMirrorCommand { get; }

        // Событие для анимации
        public event Action<string> ShowAnimationRequested;

        public MainViewModel()
        {
            BlockCommand = new Command(OnBlockButtonClicked);
            BuyVpnCommand = new Command(OnBuyVpnClicked);
            BuyMirrorCommand = new Command(OnBuyMirrorClicked);

            // Настройка таймера для случайных событий
            _timer = new Timer(GetRandomInterval());
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            // Настройка таймера для пассивного дохода
            _passiveIncomeTimer = new Timer(1000); // Каждую секунду
            _passiveIncomeTimer.Elapsed += OnPassiveIncomeTimerElapsed;
            _passiveIncomeTimer.AutoReset = true;
            _passiveIncomeTimer.Enabled = true;
        }

        public void OnBlockButtonClicked()
        {
            Traffic += TrafficPerClick;
            CheckWinCondition();
            ButtonAnimationRequested?.Invoke("BlockButtonClicked");
        }

        public void OnBuyVpnClicked()
        {
            if (Traffic >= VpnPrice)
            {
                Traffic -= VpnPrice;
                VpnCount++;
                TrafficPerClick++;
                VpnPrice += 50;
                AddNotification($"Куплен VPN! +1 трафика за клик.", isPositive: true); // Позитивное
                ShowAnimationRequested?.Invoke("VPN куплен!");
            }
            else
            {
                AddNotification("Недостаточно трафика для покупки VPN", isPositive: false); // Негативное
            }
        }

        public void OnBuyMirrorClicked()
        {
            if (Traffic >= MirrorPrice)
            {
                Traffic -= MirrorPrice;
                MirrorCount++;
                MirrorPrice += 100;
                AddNotification($"Создано зеркало! Пассивный доход увеличен.", isPositive: true); // Позитивное
                ShowAnimationRequested?.Invoke("Зеркало создано!");
            }
            else
            {
                AddNotification("Недостаточно трафика для создания зеркала", isPositive: false); // Негативное
            }
        }

        private void UpdateProgress()
        {
            Progress = (double)Traffic / TargetTraffic;
        }

        private void AddNotification(string message, bool isPositive)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Notifications.Insert(0, new Notification { Message = $"[{DateTime.Now:HH:mm:ss}] {message}", IsPositive = isPositive });
                if (Notifications.Count > 10)
                {
                    Notifications.RemoveAt(Notifications.Count - 1);
                }
            });
        }

        private void CheckWinCondition()
        {
            if (Traffic >= LevelTarget)
            {
                Level++;
                LevelTarget *= 2;
                Traffic = 0;
                VpnCount = 0;
                MirrorCount = 0;
                VpnPrice = 100;
                MirrorPrice = 500;
                AddNotification($"Поздравляем! Вы достигли уровня {Level}. Новая цель: {LevelTarget} трафика.", isPositive: true);
            }
        }

        private int GetRandomInterval()
        {
            return _random.Next(10000, 20000);
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (Traffic >= 50)
            {
                int trafficChange = 0;
                string message = "";
                bool isPositive = false;

                // Вероятность позитивных и негативных событий
                double positiveChance = 40 - (Traffic / 10) - (Level * 2); // Уменьшаем шанс на позитивное событие с каждым уровнем
                double negativeChance = 60 + (Traffic / 10) + (Level * 2); // Увеличиваем шанс на негативное событие с каждым уровнем

                double randomValue = _random.NextDouble() * 100;

                if (randomValue < positiveChance)
                {
                    // Позитивное событие
                    isPositive = true;
                    int eventType = _random.Next(0, 4);
                    switch (eventType)
                    {
                        case 0:
                            trafficChange = 100;
                            message = $"Поддержка пользователей! +{trafficChange} трафика";
                            break;
                        case 1:
                            trafficChange = 200;
                            message = $"Вирусное видео! +{trafficChange} трафика";
                            break;
                        case 2:
                            trafficChange = 150;
                            message = $"Рекламная кампания! +{trafficChange} трафика";
                            break;
                        case 3:
                            trafficChange = 300;
                            message = $"Пожертвование! +{trafficChange} трафика";
                            break;
                    }
                }
                else
                {
                    // Негативное событие
                    isPositive = false;
                    int eventType = _random.Next(0, 4);
                    switch (eventType)
                    {
                        case 0:
                            trafficChange = -100 - (Level * 10); // Увеличиваем потери с каждым уровнем
                            message = $"Проверка Роскомнадзора! {trafficChange} трафика";
                            break;
                        case 1:
                            trafficChange = -50 - (Level * 5); // Увеличиваем потери с каждым уровнем
                            message = $"Хакерская атака! {trafficChange} трафика";
                            break;
                        case 2:
                            trafficChange = -75 - (Level * 7); // Увеличиваем потери с каждым уровнем
                            message = $"Технические работы! {trafficChange} трафика";
                            break;
                        case 3:
                            trafficChange = -60 - (Level * 6); // Увеличиваем потери с каждым уровнем
                            message = $"Сбой сервера! {trafficChange} трафика";
                            break;
                    }
                }

                Traffic += trafficChange;
                AddNotification(message, isPositive);
                CheckWinCondition();

                if (Traffic < 0)
                {
                    _timer.Stop();
                    _passiveIncomeTimer.Stop();
                    AddNotification("Игра окончена! Ваш сайт заблокирован Роскомнадзором.", isPositive: false);
                }
            }

            // Устанавливаем новый случайный интервал
            _timer.Interval = GetRandomInterval();
        }

        private void OnPassiveIncomeTimerElapsed(object sender, ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                int passiveIncome = MirrorCount * 5; // Каждое зеркало приносит 10 трафика
                if (passiveIncome > 0)
                {
                    Traffic += passiveIncome;
                    AddNotification($"Пассивный доход: +{passiveIncome} трафика от зеркал", isPositive: true);
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}