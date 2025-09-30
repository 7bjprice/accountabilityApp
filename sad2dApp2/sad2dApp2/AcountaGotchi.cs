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
        public int Happiness { get; set; }
        public int Wellness { get; set; }

        int happinessDecayRate = 6;
        int wellnessDecayRate = 6;

        public AcountaGotchi(string name)
        {
            Name = name;
            Happiness = 75;
            Wellness = 75;
        }

        public AcountaGotchi(string name, int happiness, int wellness)
        {
            Name = name;
            Happiness = happiness;
            Wellness = wellness;
        }

        public void CompleteGoal()
        {

        }
    }
}
