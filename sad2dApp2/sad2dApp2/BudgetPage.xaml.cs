using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace sad2dApp2
{
    public partial class BudgetPage : ContentPage
    {
        // Observable collection for budget items
        public ObservableCollection<BudgetItem> BudgetItems { get; set; }

        public int _TotalBudget;

        public BudgetPage()
        {
            InitializeComponent();

            // Sample data
            BudgetItems = new ObservableCollection<BudgetItem> { };

            // Bind to CollectionView
            BudgetList.ItemsSource = BudgetItems;
        }

        private async void OnAddBudgetItemClicked(object sender, EventArgs e)
        {
            // Prompt for category
            string category = await DisplayPromptAsync("New Budget Item", "Enter category:");
            if (string.IsNullOrWhiteSpace(category))
                return;

            // Prompt for amount
            string amountStr = await DisplayPromptAsync("New Budget Item", "Enter amount:", keyboard: Keyboard.Numeric);
            if (double.TryParse(amountStr, out double amount))
            {
                BudgetItems.Add(new BudgetItem { Category = category, Amount = amount });
            }
            else
            {
                await DisplayAlert("Invalid Input", "Please enter a valid number for the amount.", "OK");
            }
        }
    
        private async void OnSetBudgetClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Set Total Budget", "Enter new total budget:", keyboard: Keyboard.Numeric);
            if (double.TryParse(result, out double newBudget) && newBudget >= 0)
            {
                _TotalBudget = (int)newBudget;
                TotalBudgetLabel.Text = $"${_TotalBudget}";
            }
            else if (!string.IsNullOrWhiteSpace(result))
            {
                await DisplayAlert("Invalid Input", "Please enter a valid number for the budget.", "OK");
            }
        }
    
            
    }

    // Simple model class
    public class BudgetItem
    {
        public string ?Category { get; set; }
        public double Amount { get; set; }
    }
}
