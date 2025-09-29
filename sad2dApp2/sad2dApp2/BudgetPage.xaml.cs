using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace sad2dApp2
{
    public partial class BudgetPage : ContentPage
    {
        // Observable collection for budget items
        public ObservableCollection<BudgetItem> BudgetItems { get; set; }

        public BudgetPage()
        {
            InitializeComponent();

            // Sample data
            BudgetItems = new ObservableCollection<BudgetItem>
            {
                new BudgetItem { Category = "Groceries", Amount = 300 },
                new BudgetItem { Category = "Rent", Amount = 1200 },
                new BudgetItem { Category = "Utilities", Amount = 150 },
                new BudgetItem { Category = "Entertainment", Amount = 100 }
            };

            // Bind to CollectionView
            BudgetList.ItemsSource = BudgetItems;
        }

        private async void OnAddBudgetItemClicked(object sender, EventArgs e)
        {
            // Example: Navigate to a new page or show a modal to add an item
            await DisplayAlert("Add Item", "This would open a form to add a new budget item.", "OK");
        }
    }

    // Simple model class
    public class BudgetItem
    {
        public string ?Category { get; set; }
        public double Amount { get; set; }
    }
}
