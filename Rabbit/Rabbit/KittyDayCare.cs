using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit
{
    public class KittyDayCare
    {
        public KittyDayCare(string name, string activity, DateTime timeStamp)
        {
            Name = name;
            Activity = activity;
            TimeStamp = timeStamp;
        }

        public string Name { get; set; }
        public string Activity { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
