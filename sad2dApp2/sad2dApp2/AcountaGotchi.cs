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
        public DateTime LastDailyDrop { get; set; } = DateTime.MinValue;

        public DateTime LastGoalReset { get; set; } = DateTime.MinValue;

        float hourlydecay = 6/24f; // 6 points per day

        public AcountaGotchi(string name) //This function is used when creating a tomagotchi for the first time
        {
            Name = name;
            Happiness = 100;
            Wellness = 100;
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
            {
                DateTime now = DateTime.Now;
                DateTime today = now.Date;

                // Initialize missing fields
                if (LastLogin == DateTime.MinValue)
                    LastLogin = now;

                if (LastDailyDrop == DateTime.MinValue)
                    LastDailyDrop = today;

                // ---- DAILY DEDUCTION ----
                int daysPassed = (today - LastDailyDrop.Date).Days;

                if (daysPassed > 0)
                {
                    float dailyDeduction = 10 * daysPassed;
                    Wellness = Math.Max(0, Wellness - dailyDeduction);
                    LastDailyDrop = today; // Only update once per real new day
                }

                // ---- OPTIONAL: HOURLY DECAY ----
                // Only apply decay if you *want* gradual loss during the day.
                // Comment this block out entirely if you only want daily drops.
                int hoursSinceLastLogin = (int)(now - LastLogin).TotalHours;
                if (hoursSinceLastLogin > 0 && hoursSinceLastLogin < 24) // cap within a day
                {
                    float decayAmount = hoursSinceLastLogin * hourlydecay;
                    Wellness = Math.Max(0, Wellness - decayAmount);
                }

                // Update login time
                LastLogin = now;
            }


            // int hoursSinceLastLogin = (int)(DateTime.Now - LastLogin).TotalHours;
            // float decayAmount = hoursSinceLastLogin * hourlydecay;
            // Wellness = Math.Max(0, Wellness - decayAmount);

            // DateTime today = DateTime.Now.Date;
            // int daysPassed = (today - LastDailyDrop.Date).Days;

            // if (daysPassed > 0)
            // {
            //     float dailyDeduction = 10 * daysPassed;
            //     Wellness = Math.Max(0, Wellness - dailyDeduction);
            //     LastDailyDrop = today;
            // }
            // LastLogin = DateTime.Now;
        }
        

        public void ResetStats()
        {
            Happiness = 100;
            Wellness = 100;
        }

        public void SubtractHappiness(float amount)
        {
            Happiness = Math.Max(0, Happiness - amount);
        }
        
        
    }

    public static class GotchiService
    {
        public static AcountaGotchi? Current { get; set; }

        public static event Action OnGotchiUpdated;

        public static void NotifyUpdated()
        {
            OnGotchiUpdated?.Invoke();
        }
        
    }
}
