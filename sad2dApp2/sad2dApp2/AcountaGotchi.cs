using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace sad2dApp2
{
    public class AcountaGotchi
    {
        public string Name { get; set; }
        public float Happiness { get; set; } // Finacial
        public float Wellness { get; set; } // Goals
        public DateTime StartDate { get; set; }
        public DateTime LastLogin { get; set; }

        float hourlydecay = 6/24f; // 6 points per day

        public AcountaGotchi(string name) //This function is used when creating a tomagotchi for the first time
        {
            Name = name;
            Happiness = 100;
            Wellness = 75;
            StartDate = DateTime.Now;
            LastLogin = DateTime.Now;
            SaveSystem.SaveAcountagotchiToFileAsync(name, this);
        }

        [JsonConstructor]
        public AcountaGotchi(string name, float happiness, float wellness, DateTime startDate, DateTime lastLogin)
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
            Wellness = Math.Max(0, Wellness - decayAmount);

            LastLogin = DateTime.Now;
        }

        public void ResetStats()
        {
            Happiness = 100;
            Wellness = 75;
        }

        public void SubtractHappiness(float amount)
        {
            Happiness = Math.Max(0, Happiness - amount);
        }
    }

    public static class GotchiService
    {
        public static AcountaGotchi? Current { get; set; }
    }
}
