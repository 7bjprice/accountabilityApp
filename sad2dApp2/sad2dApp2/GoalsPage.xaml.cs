using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using System.Timers;

namespace sad2dApp2
{
    public partial class GoalsPage : ContentPage
    {
        private readonly string[] _quotes = new string[]
        {
            "Setting goals is a worthy endeavor. — Dieter F. Uchtdorf",
            "Our personal goals can bring out the best in us. — Dieter F. Uchtdorf",
            "Greatness is not always a matter of the scale of one’s life, but of the quality of one’s life — Spencer W. Kimball",
            "Create a masterpiece of your life. — Joseph B. Wirthlin",
            "Ironically, procrastination produces a heavy burden laced with guilt. — Donald L. Hallstrom"
        };

        private int _currentQuoteIndex = 0;
        private System.Timers.Timer _quoteTimer;

        // Three separate collections
        public ObservableCollection<GoalsItem> DailyGoals { get; set; } = new();
        public ObservableCollection<GoalsItem> WeeklyGoals { get; set; } = new();
        public ObservableCollection<GoalsItem> MonthlyGoals { get; set; } = new();

        public GoalsPage()
        {
            InitializeComponent();
            DailyGoalsList.ItemsSource = DailyGoals;
            WeeklyGoalsList.ItemsSource = WeeklyGoals;
            MonthlyGoalsList.ItemsSource = MonthlyGoals;

            StartQuoteRotation();
        }

        // Quote rotation logic
        private void StartQuoteRotation()
        {
            QuoteLabel.Text = _quotes[_currentQuoteIndex];
            _quoteTimer = new System.Timers.Timer(7000);
            _quoteTimer.Elapsed += (s, e) => RotateQuote();
            _quoteTimer.AutoReset = true;
            _quoteTimer.Start();
        }

        private void RotateQuote()
        {
            _currentQuoteIndex = (_currentQuoteIndex + 1) % _quotes.Length;
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

        // Updated Add Goal logic
        private async void OnAddGoalsItemClicked(object sender, EventArgs e)
        {
            string goalText = await DisplayPromptAsync("New Goal", "Enter your goal:");
            if (string.IsNullOrWhiteSpace(goalText)) return;

            string[] options = { "Daily", "Weekly", "Monthly" };
            string goalType = await DisplayActionSheet("Select Goal Type", "Cancel", null, options);

            if (goalType == "Cancel" || string.IsNullOrWhiteSpace(goalType)) return;

            GoalsItem newGoal = new GoalsItem { Category = goalText };

            switch (goalType)
            {
                case "Daily":
                    DailyGoals.Add(newGoal);
                    break;
                case "Weekly":
                    WeeklyGoals.Add(newGoal);
                    break;
                case "Monthly":
                    MonthlyGoals.Add(newGoal);
                    break;
            }
        }

        private async void OnSetGoalsClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Set Goal", "Enter your why:");
            if (!string.IsNullOrWhiteSpace(result))
                TotalGoalsLabel.Text = $"Your Why: {result}";
            else
                await DisplayAlert("Invalid Input", "Please enter a valid why.", "OK");
        }

        private async void OnBudgetClicked(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync("///BudgetPage");

        private async void OnGoalsClicked(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync("///GoalsPage");

        private async void OnMainClicked(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync("///MainPage");
    }

    public class GoalsItem
    {
        public string? Category { get; set; }
    }
}
