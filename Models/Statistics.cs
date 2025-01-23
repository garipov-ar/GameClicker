using SQLite;
using System;
using Game.Services;
using System.Text.Json;

namespace Game.Models
{
    public class Statistics
    {
        public int Id { get; set; }
        public int MaxLevel { get; set; }
        public long TotalTrafficEarned { get; set; }
        public int TotalUpgradesPurchased { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}