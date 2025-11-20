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
        // private async Task ApplyDailyDeductionAsync()
        // {
        //     if (GotchiService.Current == null) return;

        //     var gotchi = GotchiService.Current;
        //     DateTime today = DateTime.Now.Date;
        //     int daysPassed = (today - gotchi.LastDailyDrop.Date).Days;

        //     if (daysPassed > 0)
        //     {
        //         float pointsToDeduct = 10 * daysPassed;
        //         gotchi.Wellness = Math.Max(0, gotchi.Wellness - pointsToDeduct);

        //         // Update LastDailyDrop to today
        //         gotchi.LastDailyDrop = today;

        //         GotchiService.NotifyUpdated();
        //         await SaveSystem.SaveAcountagotchiToFileAsync(gotchi.Name, gotchi);
        //     }
        // }
        private async void OnSimulateDayClicked(object sender, EventArgs e)
        {
            if (GotchiService.Current != null)
            {
                GotchiService.Current.LastDailyDrop = GotchiService.Current.LastDailyDrop.AddDays(-1);
                GotchiService.Current.UpdateStatsAfterLoad();
                GotchiService.NotifyUpdated();
                await SaveSystem.SaveAcountagotchiToFileAsync(GotchiService.Current.Name, GotchiService.Current);
            }
        }


        // Simulate 1 day passing
        // private async void OnSimulateDayClicked(object sender, EventArgs e)
        // {
        //     if (GotchiService.Current != null)
        //     {
        //         // Move LastDailyDrop back by 1 day to simulate a day passing
        //         GotchiService.Current.LastDailyDrop = GotchiService.Current.LastDailyDrop.AddDays(-1);

        //         // Apply deduction as if a day passed
        //         await ApplyDailyDeductionAsync();
        //     }
        // }

        // // Reset LastDailyDrop to today
        // private async void OnResetToTodayClicked(object sender, EventArgs e)
        // {
        //     if (GotchiService.Current != null)
        //     {
        //         GotchiService.Current.LastDailyDrop = DateTime.Now.Date;

        //         GotchiService.NotifyUpdated();
        //         await SaveSystem.SaveAcountagotchiToFileAsync(
        //             GotchiService.Current.Name,
        //             GotchiService.Current
        //         );
        //     }
        // }


        private async void OnTestDropClicked(object sender, EventArgs e)
        {
            if (GotchiService.Current != null)
            {
                GotchiService.Current.Wellness -= 10;
                if (GotchiService.Current.Wellness < 0)
                    GotchiService.Current.Wellness = 0;

                GotchiService.NotifyUpdated();

                await SaveSystem.SaveAcountagotchiToFileAsync(
                    GotchiService.Current.Name,
                    GotchiService.Current
                );
            }
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

            SaveGoalsItems();
        }
        protected override async void OnAppearing()
{
    base.OnAppearing();
    _quoteTimer?.Start();

    // Load saved goals
    var (items, totalGoals) = await SaveSystem.LoadGoalsItems();

    DailyGoals.Clear();
    WeeklyGoals.Clear();
    MonthlyGoals.Clear();

    foreach (var item in items)
    {
        switch (item.Target)
        {
            case 1: DailyGoals.Add(item); break;   // Daily
            case 7: WeeklyGoals.Add(item); break;  // Weekly
            case 30: MonthlyGoals.Add(item); break; // Monthly
            default: DailyGoals.Add(item); break;
        }
    }

    // --- DAILY GOAL RESET CHECK (new clean block) ---
    var gotchi = GotchiService.Current;
    if (gotchi != null)
    {
        DateTime today = DateTime.Now.Date;

        if (gotchi.LastGoalReset != today)
        {
            // Reset daily goals only
            foreach (var goal in DailyGoals)
                goal.IsCompleted = false;

            gotchi.LastGoalReset = today;

            // Save the gotchi
            await SaveSystem.SaveAcountagotchiToFileAsync(gotchi.Name, gotchi);

            // Save updated goal completion flags
            SaveGoalsItems();

            RefreshGoalsLists();
        }
    }
}

        // protected override async void OnAppearing()
        // {
        //     base.OnAppearing();
        //     _quoteTimer?.Start();

        //     // Load goals from JSON
        //     var (items, totalGoals) = await SaveSystem.LoadGoalsItems();

        //     // Clear current collections
        //     DailyGoals.Clear();
        //     WeeklyGoals.Clear();
        //     MonthlyGoals.Clear();

        //     // Distribute loaded items into the right collections
        //     foreach (var item in items)
        //     {
        //         switch (item.Target)
        //         {
        //             case 1:
        //                 DailyGoals.Add(item);
        //                 break;
        //             case 7:
        //                 WeeklyGoals.Add(item);
        //                 break;
        //             case 30:
        //                 MonthlyGoals.Add(item);
        //                 break;
        //             default:
        //                 DailyGoals.Add(item); // fallback
        //                 break;
        //         }

        //          var gotchi = GotchiService.Current;
        //             if (gotchi != null)
        //             {
        //                 DateTime today = DateTime.Now.Date;

        //                 if (gotchi.LastGoalReset != today)
        //                 {
        //                     // Reset daily goals only
        //                     foreach (var goal in DailyGoals)
        //                         goal.IsCompleted = false;

        //                     gotchi.LastGoalReset = today;

        //                     // Save the gotchi
        //                     await SaveSystem.SaveAcountagotchiToFileAsync(gotchi.Name, gotchi);

        //                     // Save updated goal completion flags
        //                     SaveGoalsItems();

        //                     RefreshGoalsLists();
        //                 }
        //             }
        //     }
            // if (GotchiService.Current != null)
            // {
            //     // Default LastDailyDrop to today if first load
            //     if (GotchiService.Current.LastDailyDrop == DateTime.MinValue)
            //         GotchiService.Current.LastDailyDrop = DateTime.Now.Date;

            //     // Apply daily deduction for each day passed
            //     DateTime today = DateTime.Now.Date;
            //     int daysPassed = (today - GotchiService.Current.LastDailyDrop.Date).Days;

            //     if (daysPassed > 0)
            //     {
            //         float pointsToDeduct = 10 * daysPassed;
            //         GotchiService.Current.Wellness = Math.Max(0, GotchiService.Current.Wellness - pointsToDeduct);

            //         // Update LastDailyDrop to today
            //         GotchiService.Current.LastDailyDrop = today;

            //         GotchiService.NotifyUpdated();

            //         // Save updated Gotchi
            //         await SaveSystem.SaveAcountagotchiToFileAsync(
            //             GotchiService.Current.Name,
            //             GotchiService.Current
            //         );
            //     }
            // }
        

        private async void SaveGoalsItems()
        {
            // Combine all collections into a single list
            var allGoals = DailyGoals.Concat(WeeklyGoals).Concat(MonthlyGoals).ToList();
            double totalGoals = allGoals.Count;

            await SaveSystem.SaveGoalsItems(new ObservableCollection<GoalsItem>(allGoals), totalGoals);
        }

        // Updated Add Goal logic
        private async void OnAddGoalsItemClicked(object sender, EventArgs e)
        {
            string goalText = await DisplayPromptAsync("New Goal", "Enter your goal:");
            if (string.IsNullOrWhiteSpace(goalText)) return;

            string[] options = { "Daily", "Weekly", "Monthly" };
            string goalType = await DisplayActionSheet("Select Goal Type", "Cancel", null, options);

            if (goalType == "Cancel" || string.IsNullOrWhiteSpace(goalType)) return;

            GoalsItem newGoal = new GoalsItem { Category = goalText, IsCompleted = false };

            switch (goalType)
            {
                case "Daily":
                    newGoal.Target = 1;
                    DailyGoals.Add(newGoal);
                    break;
                case "Weekly":
                    newGoal.Target = 7;
                    WeeklyGoals.Add(newGoal);
                    break;
                case "Monthly":
                    newGoal.Target = 30;
                    MonthlyGoals.Add(newGoal);
                    break;
            }
        }

        private async void OnRenameGoalClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is GoalsItem goal)
            {
                string newName = await DisplayPromptAsync("Rename Goal", "Enter new name:", initialValue: goal.Category);

                if (!string.IsNullOrWhiteSpace(newName))
                {
                    goal.Category = newName;

                    RefreshGoalsLists();
                    SaveGoalsItems();
                }
            }
        }

        private async void OnDeleteGoalClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is GoalsItem goal)
            {
                bool confirm = await DisplayAlert("Delete Goal", $"Delete '{goal.Category}'?", "Yes", "No");
                if (confirm)
                {
                    if (DailyGoals.Contains(goal)) DailyGoals.Remove(goal);
                    else if (WeeklyGoals.Contains(goal)) WeeklyGoals.Remove(goal);
                    else if (MonthlyGoals.Contains(goal)) MonthlyGoals.Remove(goal);

                    RefreshGoalsLists();
                    SaveGoalsItems();
                }
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
        private void RefreshGoalsLists()
        {
            DailyGoalsList.ItemsSource = null;
            DailyGoalsList.ItemsSource = DailyGoals;

            WeeklyGoalsList.ItemsSource = null;
            WeeklyGoalsList.ItemsSource = WeeklyGoals;

            MonthlyGoalsList.ItemsSource = null;
            MonthlyGoalsList.ItemsSource = MonthlyGoals;
        }
        

        private async void OnBudgetClicked(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync("///BudgetPage");

        private async void OnGoalsClicked(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync("///GoalsPage");

        private async void OnMainClicked(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync("///MainPage");
        private async void OnGoalCompleteClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is GoalsItem goal)
            {
                // goal.Increment();

                // DailyGoalsList.ItemsSource = null;
                // DailyGoalsList.ItemsSource = DailyGoals;
                // WeeklyGoalsList.ItemsSource = null;
                // WeeklyGoalsList.ItemsSource = WeeklyGoals;
                // MonthlyGoalsList.ItemsSource = null;
                // MonthlyGoalsList.ItemsSource = MonthlyGoals;
                goal.IsCompleted = !goal.IsCompleted;
                if (goal.IsCompleted && GotchiService.Current != null)
                {
                    // Add wellness points
                    GotchiService.Current.Wellness += 3;

                    // Cap wellness at 100
                    if (GotchiService.Current.Wellness > 100)
                        GotchiService.Current.Wellness = 100;

                    // Notify other pages that the Gotchi was updated
                    GotchiService.NotifyUpdated();

                    // Save the updated pet data to JSON
                    await SaveSystem.SaveAcountagotchiToFileAsync(
                        GotchiService.Current.Name,
                        GotchiService.Current
                    );
                }

                RefreshGoalsLists();
                SaveGoalsItems();
            }
        }
    }

}


public class GoalsItem
{
    public string? Category { get; set; }
    public int Current { get; set; }
    public int Target { get; set; }

    // public double ProgressValue => (double)Current / Math.Max(Target, 1);
    // public string Progress => $"{Current}/{Target}";
    public bool IsCompleted { get; set; } = false;
    public bool IsWhy { get; set; } = false;

    // public string StatusText => IsCompleted ? "Completed" : "Not Completed";
    public double ProgressValue => IsCompleted ? 1 : 0;
    public void Increment()
    {
        if (Current < Target)
            Current++;
    }

}
