using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using System.Timers;

namespace sad2dApp2
{
    public partial class GoalsPage : ContentPage
    {
        // Fields and properties
        private readonly string[] _quotes = new string[]
        {
            "Believe you can and you're halfway there. — Theodore Roosevelt",
            "The future depends on what you do today. — Mahatma Gandhi",
            "Don’t watch the clock; do what it does. Keep going. — Sam Levenson",
            "Success is the sum of small efforts repeated day in and day out. — Robert Collier",
            "Dream big and dare to fail. — Norman Vaughan"
        };

        private int _currentQuoteIndex = 0;
        private System.Timers.Timer _quoteTimer;


        // Existing fields
        public ObservableCollection<GoalsItem> GoalsItems { get; set; }
        public int _TotalGoals;

        public GoalsPage()
        {
            InitializeComponent();

            GoalsItems = new ObservableCollection<GoalsItem>();
            GoalsList.ItemsSource = GoalsItems;

            StartQuoteRotation();
        }

        // ✅ Quote rotation logic
        private void StartQuoteRotation()
        {
            QuoteLabel.Text = _quotes[_currentQuoteIndex];

            _quoteTimer = new System.Timers.Timer(7000);
            _quoteTimer.Elapsed += (sender, e) => RotateQuote();
            _quoteTimer.AutoReset = true;
            _quoteTimer.Start();
        }

        private void RotateQuote()
        {
            _currentQuoteIndex = (_currentQuoteIndex + 1) % _quotes.Length;

            // Use Dispatcher for UI thread updates
            Dispatcher.Dispatch(() =>
            {
                QuoteLabel.FadeTo(0, 250);
                QuoteLabel.Text = _quotes[_currentQuoteIndex];
                QuoteLabel.FadeTo(1, 250);
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _quoteTimer?.Stop();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _quoteTimer?.Start();
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
            string result = await DisplayPromptAsync("Set Goal", "Enter your why:", keyboard: Keyboard.Default);

            if (!string.IsNullOrWhiteSpace(result))
            {
                // Save the text goal
                TotalGoalsLabel.Text = $"Your Why: {result}";
            }
            else
            {
                await DisplayAlert("Invalid Input", "Please enter a valid why.", "OK");
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
