using CommunityToolkit.Maui;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;

namespace sad2dApp2
{
    public partial class MainPage : ContentPage
    {
        private const int DailyReminderNotificationId = 100;
        private const int ResetNotificationId = 1;

        public MainPage()
        {
            InitializeComponent();
            _ = InitializeAsync();
        }

        // ---------------------------
        // Lifecycle Methods
        // ---------------------------
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (GotchiService.Current != null)
            {
                GotchiService.OnGotchiUpdated += UpdateBars;
                UpdateBars();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            GotchiService.OnGotchiUpdated -= UpdateBars;
        }

        // ---------------------------
        // Initialization
        // ---------------------------
        private async Task InitializeAsync()
        {
            try
            {
                await Task.WhenAll(
                    InitializeGotchiAsync(),
                    LoadLocalHtmlAsync()
                );

                await OnAppOpenedAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Initialization Error: {ex}");
                await DisplayAlert("Error", "Failed to initialize app. Please restart.", "OK");
            }
        }

        private async Task InitializeGotchiAsync()
        {
            try
            {
                var gotchiNames = await SaveSystem.GetAllAcountaGotchiNamesAsync();

                if (gotchiNames == null || gotchiNames.Count == 0)
                {
                    var newGotchi = new AcountaGotchi("Default");
                    GotchiService.Current = newGotchi;
                    await SaveSystem.SaveAcountagotchiToFileAsync(newGotchi.Name, newGotchi);
                }
                else
                {
                    GotchiService.Current =
                        await SaveSystem.LoadAcountagotchiAsync(gotchiNames[0]);
                }

                UpdateBars();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"InitializeGotchi Error: {ex}");
                GotchiService.Current = new AcountaGotchi("Default");
                UpdateBars();
            }
        }

        // ---------------------------
        // Load local HTML into WebView
        // ---------------------------
        private async Task LoadLocalHtmlAsync()
        {
            try
            {
                string fileName =
#if ANDROID
                    "android.html";
#else
                    "windows.html";
#endif
                using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                using var reader = new StreamReader(stream);
                string htmlContent = await reader.ReadToEndAsync();

                myWebView.Source = new HtmlWebViewSource { Html = htmlContent };


            }
            catch (FileNotFoundException)
            {
                myWebView.Source = new HtmlWebViewSource
                {
                    Html = "<html><body><h1>Welcome to AccountaGotchi</h1></body></html>"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadLocalHtml Error: {ex}");
            }
        }

        // ---------------------------
        // Update UI
        // ---------------------------
        public void UpdateBars()
        {
            if (GotchiService.Current == null) return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                HappinessBar.Progress = Math.Clamp(GotchiService.Current.Happiness / 100f, 0f, 1f);
                WellnessBar.Progress = Math.Clamp(GotchiService.Current.Wellness / 100f, 0f, 1f);
            });

            _ = UpdateScoreInWebViewAsync(
                (GotchiService.Current.Happiness + GotchiService.Current.Wellness) / 2
            );
        }

        private async Task UpdateScoreInWebViewAsync(float score)
        {
            if (myWebView != null)
            {
                string js = $"updateCurrentScore({score});";
                await myWebView.EvaluateJavaScriptAsync(js);
            }
        }


        // ---------------------------
        // Navigation buttons
        // ---------------------------
        private async void OnBudgetClicked(object sender, EventArgs e) =>
            await SafeNavigateAsync("///BudgetPage", "Budget page");

        private async void OnGoalsClicked(object sender, EventArgs e) =>
            await SafeNavigateAsync("///GoalsPage", "Goals page");

        private async void OnFullScreenClicked(object sender, EventArgs e) =>
            await SafeNavigateAsync("///WebViewFullScreen", "full screen view");

        private async Task SafeNavigateAsync(string route, string pageName)
        {
            try
            {
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation Error: {ex}");
                await DisplayAlert("Error", $"Failed to navigate to {pageName}.", "OK");
            }
        }

        // ---------------------------
        // Reset data button
        // ---------------------------
        private async void OnResetClicked(object sender, EventArgs e)
        {
            try
            {
                bool confirm = await DisplayAlert(
                    "Reset Data",
                    "Are you sure you want to reset all data? This cannot be undone.",
                    "Yes, Reset",
                    "Cancel"
                );

                if (!confirm) return;

                await SaveSystem.DeleteAllSaveFilesAsync();

                var newGotchi = new AcountaGotchi("Default");
                GotchiService.Current = newGotchi;
                await SaveSystem.SaveAcountagotchiToFileAsync(newGotchi.Name, newGotchi);

                UpdateBars();

                var request = new NotificationRequest
                {
                    NotificationId = ResetNotificationId,
                    Title = "Reset Complete",
                    Description = "All data has been reset.",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(5)
                    }
                };

                await LocalNotificationCenter.Current.Show(request);

                await DisplayAlert("Success", "All data has been reset.", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Reset Error: {ex}");
                await DisplayAlert("Error", "Failed to reset data.", "OK");
            }
        }

        // ---------------------------
        // Daily Reminder
        // ---------------------------
        private async Task OnAppOpenedAsync()
        {
            try
            {
                Preferences.Set("LastOpened", DateTime.UtcNow.ToString("o"));
                await EnsureDailyReminderScheduledAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OnAppOpened Error: {ex}");
            }
        }

        private async Task EnsureDailyReminderScheduledAsync()
        {
            try
            {
                // Cancel any existing reminder to avoid duplicates
                LocalNotificationCenter.Current.Cancel(DailyReminderNotificationId);

                var request = new NotificationRequest
                {
                    NotificationId = DailyReminderNotificationId,
                    Title = "We miss you!",
                    Description = "Come check in with your AccountaGotchi 💖",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddDays(1),
                        NotifyRepeatInterval = TimeSpan.FromDays(1)
                    }
                };

                await LocalNotificationCenter.Current.Show(request);
                Debug.WriteLine($"Daily reminder scheduled for {request.Schedule.NotifyTime}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EnsureDailyReminder Error: {ex}");
            }
        }
    }
}
