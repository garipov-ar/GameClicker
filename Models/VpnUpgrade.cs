using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.ViewModels;

namespace Game.Models
{
    public class VpnUpgrade : Upgrade
    {
        public VpnUpgrade()
        {
            Name = "VPN";
            Description = "Увеличивает трафик за клик.";
            Price = 100; // Начальная цена
        }

        public override void Apply(MainViewModel viewModel)
        {
            if (viewModel.Traffic >= Price)
            {
                viewModel.Traffic -= Price;
                Count++;
                viewModel.TrafficPerClick++;
                Price += 50; // Увеличиваем цену после покупки
                viewModel.AddNotification($"Куплен {Name}! +1 трафика за клик.", isPositive: true);
            }
            else
            {
                viewModel.AddNotification($"Недостаточно трафика для покупки {Name}.", isPositive: false);
            }
        }
    }
}
