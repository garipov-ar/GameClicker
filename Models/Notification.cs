using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models
{
    public class Notification
    {
        public string Message { get; set; } = string.Empty;
        public bool IsPositive { get; set; }

        public Notification() { }
    }
}
