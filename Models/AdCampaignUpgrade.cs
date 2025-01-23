using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.ViewModels;

namespace Game.Models
{
    public class AdCampaignUpgrade : Upgrade
    {
        public AdCampaignUpgrade()
        {
            Name = "Рекламная кампания";
            Description = "Увеличивает пассивный доход на 50.";
            Price = 5000;
        }

        public override void Apply(MainViewModel viewModel)
        {
            if (viewModel.Traffic >= Price)
            {
                viewModel.Traffic -= Price;
                Count++;
                Price += 1000;
                viewModel.AddNotification($"Запущена {Name}! Пассивный доход увеличен на 50.", isPositive: true);
            }
            else
            {
                viewModel.AddNotification($"Недостаточно трафика для запуска {Name}.", isPositive: false);
            }
        }
    }
}
