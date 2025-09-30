using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sad2dApp2
{
    class AcountaGotchi
    {
        public string Name { get; set; }
        public float Happiness { get; set; }
        public float Wellness { get; set; }
        public DateTime StartDate { get; }
        public DateTime LastLogin { get; set; }

        float hourlydecay = 0.25f;

        public AcountaGotchi(string name) //This function is used when creating a tomagotchi for the first time
        {
            Name = name;
            Happiness = 75;
            Wellness = 75;
            StartDate = DateTime.Now;
            LastLogin = DateTime.Now;
        }

        public AcountaGotchi(string name, float happiness, float wellness, DateTime startDate, DateTime lastLogin) //Used when loading it from json
        {
            Name = name;
            Happiness = happiness;
            Wellness = wellness;
            StartDate = startDate;
            LastLogin = lastLogin;
            UpdateStatsAfterLoad();
        }

        public void UpdateStatsAfterLoad()
        {
            int hoursSinceLastLogin = (int)(DateTime.Now - LastLogin).TotalHours;
            float decayAmount = hoursSinceLastLogin * hourlydecay;
            Happiness = Math.Max(0, Happiness - decayAmount);
            Wellness = Math.Max(0, Wellness - decayAmount);

            LastLogin = DateTime.Now;
        }
    }
}
