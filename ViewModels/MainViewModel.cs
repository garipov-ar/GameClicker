using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Timer = System.Timers.Timer;
using Microsoft.Maui.ApplicationModel;
using System.Timers;
using Game.Models;
using System.Diagnostics;

namespace Game.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _traffic = 0;
        private int _trafficPerClick = 1;
        private double _progress;
        private ObservableCollection<Notification> _notifications = new ObservableCollection<Notification>();
        
        private const int InitialTargetTraffic = 1000;
        private const int InitialVpnPrice = 100;
        private const int InitialMirrorPrice = 1000;
        private Timer _timer;
        private Timer _passiveIncomeTimer;
        private Random _random = new Random();
        private int _level = 1;
        private int _levelTarget = InitialTargetTraffic;
        // Добавляем свойства для привязки
        public int VpnCount => Upgrades.FirstOrDefault(u => u is VpnUpgrade)?.Count ?? 0;
        public int MirrorCount => Upgrades.FirstOrDefault(u => u is MirrorUpgrade)?.Count ?? 0;
        public int VpnPrice => Upgrades.FirstOrDefault(u => u is VpnUpgrade)?.Price ?? 0;
        public int MirrorPrice => Upgrades.FirstOrDefault(u => u is MirrorUpgrade)?.Price ?? 0;
        public event Action<string>? ButtonAnimationRequested;
        public event Action? GameOverRequested;
        public event Action<string>? ShowAnimationRequested;


        public ICommand BlockCommand { get; }
        public ICommand BuyVpnCommand { get; }
        public ICommand BuyMirrorCommand { get; }

        public ObservableCollection<Upgrade> Upgrades { get; set; } = new ObservableCollection<Upgrade>
        {
            new VpnUpgrade(),
            new MirrorUpgrade(),
            new AdCampaignUpgrade()
        };

        public int PassiveIncome => GetPassiveIncome();

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
                
            }
        }

        public MainViewModel()
        {
            _timer = new Timer();
            _passiveIncomeTimer = new Timer();
            BlockCommand = new Command(OnBlockButtonClicked);
            BuyVpnCommand = new Command(OnBuyVpnClicked);
            BuyMirrorCommand = new Command(OnBuyMirrorClicked);
            Notifications = new ObservableCollection<Notification>();

            InitializeTimers();

            foreach (var upgrade in Upgrades)
            {
                upgrade.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(Upgrade.Count) || args.PropertyName == nameof(Upgrade.Price))
                    {
                        OnPropertyChanged(nameof(VpnCount));
                        OnPropertyChanged(nameof(MirrorCount));
                        OnPropertyChanged(nameof(VpnPrice));
                        OnPropertyChanged(nameof(MirrorPrice));
                    }
                };
            }
        }

        private void InitializeTimers()
        {
            _timer = new Timer(GetRandomInterval());
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            _passiveIncomeTimer = new Timer(1000); // Каждую секунду
            _passiveIncomeTimer.Elapsed += OnPassiveIncomeTimerElapsed;
            _passiveIncomeTimer.AutoReset = true;
            _passiveIncomeTimer.Enabled = true;
        }



        private void OnBuyVpnClicked()
        {
            try
            {
                var vpnUpgrade = Upgrades.FirstOrDefault(u => u is VpnUpgrade);
                if (vpnUpgrade != null)
                {
                    vpnUpgrade.Apply(this);
                }
                else
                {
                    Debug.WriteLine("Ошибка: улучшение VPN не найдено.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в OnBuyVpnClicked: {ex.Message}");
            }
        }

        private void OnBuyMirrorClicked()
        {
            var mirrorUpgrade = Upgrades.FirstOrDefault(u => u is MirrorUpgrade);
            if (mirrorUpgrade != null)
            {
                mirrorUpgrade.Apply(this);
            }
            else
            {
                Debug.WriteLine("Ошибка: улучшение Mirror не найдено.");
            }
        }

        public void OnBlockButtonClicked()
        {
            Traffic += TrafficPerClick;
            CheckWinCondition();
            ButtonAnimationRequested?.Invoke("BlockButtonClicked");
        }

        private int GetPassiveIncome()
        {
            int income = 0;
            foreach (var upgrade in Upgrades)
            {
                if (upgrade is MirrorUpgrade mirrorUpgrade)
                {
                    income += mirrorUpgrade.Count * 10; 
                }
                else if (upgrade is AdCampaignUpgrade adCampaignUpgrade)
                {
                    income += adCampaignUpgrade.Count * 50;
                }
            }
            return income;
        }

        private void UpdateProgress()
        {
            Progress = (double)Traffic / LevelTarget;
        }

        public void AddNotification(string message, bool isPositive)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var notification = new Notification
                {
                    Message = $"[{DateTime.Now:HH:mm:ss}] {(isPositive ? "[+]" : "[-]")} {message}",
                    IsPositive = isPositive
                };

                Notifications.Insert(0, notification);

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
                LevelTarget = (int)(LevelTarget * 1.5);
                // Удалить: Traffic = 0;
                ResetUpgrades();
                AddNotification($"Поздравляем! Уровень {Level}. Новая цель: {LevelTarget} трафика. Ваш текущий трафик: {Traffic}", isPositive: true);
            }
        }

        private void ResetUpgrades()
        {
            foreach (var upgrade in Upgrades)
            {
                upgrade.Count = 0; 
                                   
                if (upgrade is VpnUpgrade vpn)
                {
                    vpn.Price = InitialVpnPrice;
                }
                else if (upgrade is MirrorUpgrade mirror)
                {
                    mirror.Price = InitialMirrorPrice;
                }
                else if (upgrade is AdCampaignUpgrade ad)
                {
                    ad.Price = 5000; 
                }
            }
        }

        private int GetRandomInterval()
        {
            return _random.Next(10000, 20000);
        }

        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Traffic >= 50)
                    {
                        HandleRandomEvent();
                    }
                    _timer.Interval = GetRandomInterval();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в OnTimedEvent: {ex.Message}");
            }
        }

        private void HandleRandomEvent()
        {
            int trafficChange = 0;
            string message = "";
            bool isPositive = false;

            double positiveChance = Math.Max(10, 50 - Level * 2);
            double randomValue = _random.NextDouble() * 100;

            if (randomValue < positiveChance)
            {
                // Позитивное событие
                isPositive = true;
                trafficChange = GetPositiveTrafficChange();
                message = GetPositiveEventMessage(trafficChange);
            }
            else
            {
                // Негативное событие
                isPositive = false;
                trafficChange = GetNegativeTrafficChange();
                message = GetNegativeEventMessage(trafficChange);
            }

            Traffic += trafficChange;
            AddNotification(message, isPositive);

            CheckWinCondition();

            if (Traffic < 0)
            {
                GameOver();
            }
        }

        private int GetPositiveTrafficChange()
        {
            int[] trafficChanges = { 100, 200, 150, 300, 250, 180, 220, 350 };
            return trafficChanges[_random.Next(trafficChanges.Length)];
        }

        private int GetNegativeTrafficChange()
        {
            int[] trafficChanges = { -100, -50, -75, -60, -150, -200, -120, -180 };
            return trafficChanges[_random.Next(trafficChanges.Length)] - Level * 10;
        }

        private string GetPositiveEventMessage(int trafficChange)
        {
            string[] messages =
            {
                $"Поддержка пользователей! +{trafficChange} трафика",
                $"Вирусное видео! +{trafficChange} трафика",
                $"Рекламная кампания! +{trafficChange} трафика",
                $"Пожертвование! +{trafficChange} трафика",
                $"Победа в конкурсе! +{trafficChange} трафика",
                $"Хороший отзыв крупного блогера! +{trafficChange} трафика",
                $"Попадание в новостную ленту! +{trafficChange} трафика",
                $"Сезонный всплеск популярности! +{trafficChange} трафика"
            };
            return messages[_random.Next(messages.Length)];
        }

        private string GetNegativeEventMessage(int trafficChange)
        {
            string[] messages =
            {
                $"Проверка Роскомнадзора! {trafficChange} трафика",
                $"Хакерская атака! {trafficChange} трафика",
                $"Технические работы! {trafficChange} трафика",
                $"Сбой сервера! {trafficChange} трафика",
                $"Плохой отзыв крупного блогера! {trafficChange} трафика",
                $"Массовая отписка пользователей! {trafficChange} трафика",
                $"Ошибка при обновлении сайта! {trafficChange} трафика",
                $"Снижение интереса! {trafficChange} трафика"
            };
            return messages[_random.Next(messages.Length)];
        }

        private void GameOver()
        {
            _timer.Stop();
            _passiveIncomeTimer.Stop();

            // Сбросить трафик за клик
            TrafficPerClick = 1;

            // Полный сброс всех улучшений
            ResetUpgrades();

            // Сбросить уровень и трафик
            Traffic = 0;
            Level = 1;
            LevelTarget = InitialTargetTraffic;
            Progress = 0;

            AddNotification("Game Over! Ваш сайт заблокирован Роскомнадзором.", isPositive: false);
            Notifications.Clear();

            // Обновить привязки
            OnPropertyChanged(nameof(Traffic));
            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(LevelTarget));
            OnPropertyChanged(nameof(Progress));

            GameOverRequested?.Invoke();
        }

        public void OnPassiveIncomeTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                int passiveIncome = PassiveIncome;
                if (passiveIncome > 0)
                {
                    Traffic += passiveIncome;
                    AddNotification($"Пассивный доход: +{passiveIncome} трафика от зеркал", isPositive: true);
                    // Обновляем привязку для отображения в интерфейсе
                    OnPropertyChanged(nameof(PassiveIncome));
                    OnPropertyChanged(nameof(Traffic));
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }
}