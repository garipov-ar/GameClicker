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
        private int _mirrorPrice = 1000;
        private int _level = 1;
        private int _levelTarget = 1000;
        public event Action GameOverRequested;

        public int VpnPrice
        {
            get => _vpnPrice;
            set
            {
                if (_vpnPrice != value)
                {
                    _vpnPrice = value;
                    OnPropertyChanged(); // Уведомляем об изменении
                }
            }
        }
        public int MirrorPrice
        {
            get => _mirrorPrice;
            set
            {
                if (_mirrorPrice != value)
                {
                    _mirrorPrice = value;
                    OnPropertyChanged(); // Уведомляем об изменении
                }
            }
        }
        public int PassiveIncome => MirrorCount * 10; // Пассивный доход
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
                VpnPrice += 50; // Увеличиваем стоимость VPN на 50 после покупки
                AddNotification($"Куплен VPN! +1 трафика за клик.", isPositive: true); // Позитивное
                ShowAnimationRequested?.Invoke("VPN куплен!");

                // Проверка на Game Over
                if (Traffic < 0)
                {
                    GameOver();
                }
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
                MirrorPrice += 200; // Увеличиваем стоимость зеркала на 200 после покупки
                AddNotification($"Создано зеркало! Пассивный доход увеличен.", isPositive: true); // Позитивное
                ShowAnimationRequested?.Invoke("Зеркало создано!");

                // Проверка на Game Over
                if (Traffic < 0)
                {
                    GameOver();
                }
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
            // Выполняем изменения в UI-потоке
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Notifications.Insert(0, new Notification { Message = $"[{DateTime.Now:HH:mm:ss}] {message}", IsPositive = isPositive });

                // Ограничиваем количество уведомлений в списке
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
                LevelTarget = (int)(LevelTarget * 1.5); // Увеличиваем цель на 50%
                Traffic = 0;
                VpnCount = 0;
                MirrorCount = 0;
                VpnPrice = 100;
                MirrorPrice = 1000; // Сбрасываем стоимость зеркал

                // Очищаем уведомления
                Notifications.Clear();

                AddNotification($"Поздравляем! Вы достигли уровня {Level}. Новая цель: {LevelTarget} трафика.", isPositive: true);
            }
        }

        private int GetRandomInterval()
        {
            return _random.Next(10000, 20000);
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Traffic >= 50)
                {
                    int trafficChange = 0;
                    string message = "";
                    bool isPositive = false;

                    double positiveChance = Math.Max(20, 60 - (Traffic / 10) - (Level * 2)); // Минимальный шанс 10%
                    double randomValue = _random.NextDouble() * 100;

                    if (randomValue < positiveChance)
                    {
                        // Позитивное событие
                        isPositive = true;
                        int eventType = _random.Next(0, 8); // 8 типов событий
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
                            case 4:
                                trafficChange = 250;
                                message = $"Победа в конкурсе! +{trafficChange} трафика";
                                break;
                            case 5:
                                trafficChange = 180;
                                message = $"Хороший отзыв крупного блогера! +{trafficChange} трафика";
                                break;
                            case 6:
                                trafficChange = 220;
                                message = $"Попадание в новостную ленту! +{trafficChange} трафика";
                                break;
                            case 7:
                                trafficChange = 350;
                                message = $"Сезонный всплеск популярности! +{trafficChange} трафика";
                                break;
                        }
                    }
                    else
                    {
                        // Негативное событие
                        isPositive = false;
                        int eventType = _random.Next(0, 8); // 8 типов событий
                        switch (eventType)
                        {
                            case 0:
                                trafficChange = -100 - (Level * 10);
                                message = $"Проверка Роскомнадзора! {trafficChange} трафика";
                                break;
                            case 1:
                                trafficChange = -50 - (Level * 5);
                                message = $"Хакерская атака! {trafficChange} трафика";
                                break;
                            case 2:
                                trafficChange = -75 - (Level * 7);
                                message = $"Технические работы! {trafficChange} трафика";
                                break;
                            case 3:
                                trafficChange = -60 - (Level * 6);
                                message = $"Сбой сервера! {trafficChange} трафика";
                                break;
                            case 4:
                                trafficChange = -150 - (Level * 10);
                                message = $"Плохой отзыв крупного блогера! {trafficChange} трафика";
                                break;
                            case 5:
                                trafficChange = -200;
                                message = $"Массовая отписка пользователей! {trafficChange} трафика";
                                break;
                            case 6:
                                trafficChange = -120 - (Level * 8);
                                message = $"Ошибка при обновлении сайта! {trafficChange} трафика";
                                break;
                            case 7:
                                trafficChange = -180;
                                message = $"Снижение интереса! {trafficChange} трафика";
                                break;
                        }
                    }

                    // Изменяем трафик и добавляем уведомление
                    Traffic += trafficChange;
                    AddNotification(message, isPositive);

                    // Проверка на выполнение условий выигрыша
                    CheckWinCondition();

                    // Проверка на Game Over
                    if (Traffic < 0)
                    {
                        GameOver();
                    }
                }

                // Устанавливаем новый интервал таймера
                _timer.Interval = GetRandomInterval();
            });
        }


        private void GameOver()
        {
            // Останавливаем таймеры
            _timer.Stop();
            _passiveIncomeTimer.Stop();

            // Добавляем уведомление о завершении игры
            AddNotification("Game Over! Ваш сайт заблокирован Роскомнадзором.", isPositive: false);

            // Сбрасываем состояние игры
            Traffic = 0;
            VpnCount = 0;
            MirrorCount = 0;
            TrafficPerClick = 1;
            VpnPrice = 100; // Сбрасываем стоимость VPN
            MirrorPrice = 1000; // Сбрасываем стоимость зеркал
            Level = 1;
            LevelTarget = 1000;
            Progress = 0;

            // Очищаем уведомления
            Notifications.Clear();

            // Оповещаем UI об изменении свойств
            OnPropertyChanged(nameof(Traffic));
            OnPropertyChanged(nameof(VpnCount));
            OnPropertyChanged(nameof(MirrorCount));
            OnPropertyChanged(nameof(TrafficPerClick));
            OnPropertyChanged(nameof(VpnPrice));
            OnPropertyChanged(nameof(MirrorPrice));
            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(LevelTarget));
            OnPropertyChanged(nameof(Progress));

            // Вызываем событие для перехода на страницу GameOver
            GameOverRequested?.Invoke();
        }


        private void OnPassiveIncomeTimerElapsed(object sender, ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                int passiveIncome = PassiveIncome; // Используем свойство PassiveIncome
                if (passiveIncome > 0)
                {
                    Traffic += passiveIncome;
                    AddNotification($"Пассивный доход: +{passiveIncome} трафика от зеркал", isPositive: true);
                    Console.WriteLine($"Пассивный доход: {passiveIncome}, Текущий трафик: {Traffic}");
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