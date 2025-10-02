using System;
using CommunityToolkit.Maui;

namespace sad2dApp2
{
    public partial class MainPage : ContentPage
    {
        // Make property type accessibility match property accessibility
        public AcountaGotchi? CurrentAcountaGotchi { get; set; }

        public MainPage()
        {
            InitializeComponent();

            // Load First AcountaGotchi
            var acountaGotchiNames = SaveSystem.GetAllAcountaGotchiNames();
            if (acountaGotchiNames.Count == 0)
            {
                // No AcountaGotchi found, create a new one
                var newAcountaGotchi = new AcountaGotchi("Default");
                acountaGotchiNames.Add(newAcountaGotchi.Name);
                CurrentAcountaGotchi = newAcountaGotchi;
            }
            else //Load first acountaGotchi
            {
                CurrentAcountaGotchi = SaveSystem.LoadAcountagotchi(acountaGotchiNames[0]);
            }
        }

        private async void OnBudgetClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///BudgetPage");
        }
        private async void OnGoalsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///GoalsPage");
        }
    }
}
