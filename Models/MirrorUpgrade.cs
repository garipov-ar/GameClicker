using System;
using System.Diagnostics;
using Game.ViewModels;

namespace Game.Models
{
    public class MirrorUpgrade : Upgrade
    {
        public MirrorUpgrade()
        {
            Name = "Зеркало";
            Description = "Увеличивает пассивный доход.";
            Price = 1000; 
        }

        public override void Apply(MainViewModel viewModel)
        {
            if (Count >= 10)
            {
                viewModel.AddNotification($"Достигнуто максимальное количество {Name}.", isPositive: false);
                return;
            }

            if (viewModel.Traffic >= Price)
            {
                viewModel.Traffic -= Price;
                Count++;
                Price += (int)(Price * 0.2);
                viewModel.AddNotification($"Создано {Name}! Пассивный доход увеличен на 10.", isPositive: true);
                
                viewModel.OnPropertyChanged(nameof(viewModel.PassiveIncome));
            }
            else
            {
                viewModel.AddNotification($"Недостаточно трафика для создания {Name}.", isPositive: false);
            }
        }
    }
}