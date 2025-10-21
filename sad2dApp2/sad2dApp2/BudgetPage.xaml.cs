using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace sad2dApp2
{
    public partial class BudgetPage : ContentPage
    {
        double _penaltyCoefficient = 0.5;

        public ObservableCollection<BudgetItem> BudgetItems { get; set; }
        private double _totalBudget;

        public BudgetPage()
        {
            InitializeComponent();
            BudgetItems = new ObservableCollection<BudgetItem>();
            BudgetList.ItemsSource = BudgetItems;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var (items, totalBudget) = await SaveSystem.LoadBudgetItems();
            BudgetItems.Clear();
            foreach (var item in items)
                BudgetItems.Add(item);
            _totalBudget = totalBudget;
            TotalBudgetLabel.Text = $"${_totalBudget:F2}";
            UpdateTotals();
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
            UpdateTotals();
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

                // Check if over budget

                if (item.Remaining < 0)
                {
                    await DisplayAlert("Over Budget", $"You have exceeded your budget for {item.Category}!", "OK");

                    float points_to_deduct = ((float)item.Amount / (float)_totalBudget) * (((float)(item.Spent)-(float)(item.Amount)) * (float)_penaltyCoefficient);

                    points_to_deduct = Math.Min(points_to_deduct, GotchiService.Current.Happiness);



                    // Debug
                    Debug.WriteLine($"DEBUG → totalBudget={_totalBudget}, " +
                        $"amount={item.Amount}, " +
                        $"spent={item.Spent}, " +
                        $"points_to_deduct={points_to_deduct}");

                    // Update happiness points from AcountaGotchi
                    if (GotchiService.Current != null)
                    {
                        GotchiService.Current.SubtractHappiness(points_to_deduct);
                        GotchiService.NotifyUpdated();
                        await SaveSystem.SaveAcountagotchiToFileAsync(
                            GotchiService.Current.Name,
                            GotchiService.Current
                        );
                    }
                }
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
            UpdateTotals();
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
            UpdateTotals();
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

        private async void UpdateTotals()
        {
            double totalSpent = 0;
            foreach (var item in BudgetItems)
                totalSpent += item.Spent;

            double remaining = _totalBudget - totalSpent;
            TotalSpentLabel.Text = $"Spent: ${totalSpent:F2}";
            RemainingLabel.Text = $"Remaining: ${remaining:F2}";
            await SaveSystem.SaveBudgetItems(BudgetItems, _totalBudget);
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
