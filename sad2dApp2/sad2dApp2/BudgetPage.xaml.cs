using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace sad2dApp2
{
    public partial class BudgetPage : ContentPage
    {
        public ObservableCollection<BudgetItem> BudgetItems { get; set; }
        private double _totalBudget;

        public BudgetPage()
        {
            InitializeComponent();
            BudgetItems = new ObservableCollection<BudgetItem>();
            BudgetList.ItemsSource = BudgetItems;
        }

        private async void OnAddBudgetItemClicked(object sender, EventArgs e)
        {
            string category = await DisplayPromptAsync("New Budget Item", "Enter category:");
            if (string.IsNullOrWhiteSpace(category))
                return;

            string amountStr = await DisplayPromptAsync("New Budget Item", "Enter amount:", keyboard: Keyboard.Numeric);
            if (double.TryParse(amountStr, out double amount))
            {
                BudgetItems.Add(new BudgetItem
                {
                    Category = category,
                    Amount = amount,
                    Spent = 0
                });
            }
            else
            {
                await DisplayAlert("Invalid Input", "Please enter a valid number.", "OK");
            }
        }

        private async void OnAddExpenseClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            var item = button?.BindingContext as BudgetItem;
            if (item == null) return;

            string expenseStr = await DisplayPromptAsync("Add Expense", $"Enter expense amount for {item.Category}:", keyboard: Keyboard.Numeric);

            if (double.TryParse(expenseStr, out double expense) && expense > 0)
            {
                item.Spent += expense;

                item.Remaining = item.Amount - item.Spent;
                item.Progress = Math.Min(item.Spent / item.Amount, 1);

                // Refresh UI
                BudgetList.ItemsSource = null;
                BudgetList.ItemsSource = BudgetItems;
                UpdateTotals();
            }
            else if (!string.IsNullOrWhiteSpace(expenseStr))
            {
                await DisplayAlert("Invalid Input", "Please enter a valid expense amount.", "OK");
            }
        }

        private async void OnRenameItemClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            var item = button?.BindingContext as BudgetItem;
            if (item == null) return;

            string newName = await DisplayPromptAsync("Rename Category", "Enter new name:");
            if (!string.IsNullOrWhiteSpace(newName))
            {
                item.Category = newName;
                BudgetList.ItemsSource = null;
                BudgetList.ItemsSource = BudgetItems;
            }
        }

        private async void OnDeleteItemClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            var item = button?.BindingContext as BudgetItem;
            if (item == null) return;

            bool confirm = await DisplayAlert("Delete Item", $"Remove {item.Category}?", "Yes", "No");
            if (confirm)
            {
                BudgetItems.Remove(item);
            }
        }

        private async void OnSetBudgetClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Set Total Budget", "Enter new total budget:", keyboard: Keyboard.Numeric);
            if (double.TryParse(result, out double newBudget) && newBudget >= 0)
            {
                _totalBudget = newBudget;
                TotalBudgetLabel.Text = $"${_totalBudget:F2}";
                UpdateTotals();
            }
            else if (!string.IsNullOrWhiteSpace(result))
            {
                await DisplayAlert("Invalid Input", "Please enter a valid number for the budget.", "OK");
            }
        }

        private void UpdateTotals()
        {
            double totalSpent = 0;
            foreach (var item in BudgetItems)
                totalSpent += item.Spent;

            double remaining = _totalBudget - totalSpent;
            TotalSpentLabel.Text = $"Spent: ${totalSpent:F2}";
            RemainingLabel.Text = $"Remaining: ${remaining:F2}";
        }

        private async void OnMainClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///MainPage");
        }

        private async void OnGoalsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///GoalsPage");
        }
    }

    public class BudgetItem
    {
        public string Category { get; set; }
        public double Amount { get; set; }
        public double Spent { get; set; }
        public double Remaining { get; set; }
        public double Progress { get; set; }
    }
}
