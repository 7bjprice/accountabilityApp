using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace sad2dApp2
{
    public partial class GoalsPage : ContentPage
    {
        // Observable collection for goals items
        public ObservableCollection<GoalsItem> GoalsItems { get; set; }

        public int _TotalGoals;

        public GoalsPage()
        {
            InitializeComponent();

            // Sample data
            GoalsItems = new ObservableCollection<GoalsItem> { };

            // Bind to CollectionView
            GoalsList.ItemsSource = GoalsItems;
        }

        private async void OnAddGoalsItemClicked(object sender, EventArgs e)
        {
            // Prompt for category
            string category = await DisplayPromptAsync("New Goals Item", "Enter category:");
            if (string.IsNullOrWhiteSpace(category))
                return;

            // Prompt for amount
            string amountStr = await DisplayPromptAsync("New Goals Item", "Enter amount:", keyboard: Keyboard.Numeric);
            if (double.TryParse(amountStr, out double amount))
            {
                GoalsItems.Add(new GoalsItem { Category = category, Amount = amount });

            }
            else
            {
                await DisplayAlert("Invalid Input", "Please enter a valid number for the amount.", "OK");
            }
        }

        private async void OnSetGoalsClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Set Total Goals", "Enter new total goals:", keyboard: Keyboard.Numeric);
            if (double.TryParse(result, out double newGoals) && newGoals >= 0)
            {
                _TotalGoals = (int)newGoals;
                TotalGoalsLabel.Text = $"${_TotalGoals}";
            }
            else if (!string.IsNullOrWhiteSpace(result))
            {
                await DisplayAlert("Invalid Input", "Please enter a valid number for the goals.", "OK");
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
        private async void OnMainClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///MainPage");
        }
    }
    

    // Simple model class
    public class GoalsItem
    {
        public string? Category { get; set; }
        public double Amount { get; set; }
    }
}
