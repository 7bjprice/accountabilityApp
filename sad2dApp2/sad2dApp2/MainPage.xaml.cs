using CommunityToolkit.Maui;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls; // for WebViewSource
using Plugin.LocalNotification;

namespace sad2dApp2
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<TaskItem> Tasks { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();
            _ = InitializeGotchiAsync();
            GotchiService.OnGotchiUpdated += UpdateBars;
            LoadLocalHtml();
            Preferences.Set("LastOpened", DateTime.UtcNow);
            CheckInactivityAndScheduleNotification();
            LocalNotificationCenter.Current.Cancel(100);


        }

        private async void LoadLocalHtml()
        {
            Debug.WriteLine("LoadLocalHtml started.");

            try
            {
                Debug.WriteLine("Opening local HTML file: index.html");
#if ANDROID
    // Load android.html
    using var stream = await FileSystem.OpenAppPackageFileAsync("android.html");
#else
                // Load windows.html
                using var stream = await FileSystem.OpenAppPackageFileAsync("windows.html");

#endif
                Debug.WriteLine("File stream opened successfully.");

                using var reader = new StreamReader(stream);
                string htmlContent = await reader.ReadToEndAsync();
                Debug.WriteLine("HTML content read successfully. Length: " + htmlContent.Length);

                Debug.WriteLine("Setting WebView source.");
                myWebView.Source = new HtmlWebViewSource
                {
                    Html = htmlContent
                };
                Debug.WriteLine("WebView source set successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in LoadLocalHtml: " + ex);
            }

            Debug.WriteLine("LoadLocalHtml finished.");
        }

        private async Task InitializeGotchiAsync()
        {
            var acountaGotchiNames = await SaveSystem.GetAllAcountaGotchiNamesAsync();
            Debug.WriteLine($"Found {acountaGotchiNames.Count} AcountaGotchi(s).");

            if (acountaGotchiNames.Count == 0)
            {
                // No AcountaGotchi found, create and save a new one
                var newAcountaGotchi = new AcountaGotchi("Default");
                GotchiService.Current = newAcountaGotchi;

                await SaveSystem.SaveAcountagotchiToFileAsync(newAcountaGotchi.Name, newAcountaGotchi);
                Debug.WriteLine($"Created and saved new AcountaGotchi: {newAcountaGotchi.Name}");
            }
            else
            {
                // Load the first AcountaGotchi
                var CurrentAcountaGotchi = await SaveSystem.LoadAcountagotchiAsync(acountaGotchiNames[0]);
                GotchiService.Current = CurrentAcountaGotchi;
                Debug.WriteLine($"Loaded AcountaGotchi: {CurrentAcountaGotchi?.Name}");
            }

            UpdateBars();
        }

        public void UpdateBars()
        {
            if(GotchiService.Current == null)
            {
                Debug.WriteLine("GotchiService.Current is null, cannot update bars.");
                return;
            }
            HappinessBar.Progress = GotchiService.Current.Happiness / 100f;
            WellnessBar.Progress = GotchiService.Current.Wellness / 100f;
        }

        private async void OnBudgetClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///BudgetPage");
        }
        private async void OnGoalsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///GoalsPage");
        }
        private async void OnResetClicked(object sender, EventArgs e)
        {
            var request = new NotificationRequest
            {
                Title = "Reset",
                Description = "All data has been reset.",
                NotificationId = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(5)
                }
            };
            await LocalNotificationCenter.Current.Show(request);
            await SaveSystem.DeleteAllSaveFilesAsync();
            UpdateBars();

        }
        private async void OnFullScreenClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///WebViewFullScreen");
        }

        private void ScheduleDailyNotification()
        {
            var request = new NotificationRequest
            {
                NotificationId = 100,
                Title = "We miss you!",
                Description = "Come check in with your AccountaGotchi 💖",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(5),   // first run 5 seconds from now
                    RepeatType = NotificationRepeat.Daily
                }
            };

            LocalNotificationCenter.Current.Show(request);
        }

        private void CheckInactivityAndScheduleNotification()
        {
            var lastOpened = Preferences.Get("LastOpened", DateTime.UtcNow);

            var elapsed = DateTime.UtcNow - lastOpened;

            if (elapsed >= TimeSpan.FromHours(24))
            {
                ScheduleDailyNotification();
            }
        }


    }
}
