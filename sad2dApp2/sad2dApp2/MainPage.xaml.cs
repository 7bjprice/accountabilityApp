using CommunityToolkit.Maui;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
        }

        private async void LoadLocalHtml()
        {
            // Open the local HTML file from Resources/Raw
            using var stream = await FileSystem.OpenAppPackageFileAsync("index.html");
            using var reader = new StreamReader(stream);
            string htmlContent = await reader.ReadToEndAsync();

            // Set the WebView’s source to the HTML content
            myWebView.Source = new HtmlWebViewSource
            {
                Html = htmlContent
            };
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

            LoadDailyTasks();
            UpdateBars();
            TasksList.ItemsSource = Tasks;
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

        private void LoadDailyTasks()
        {
            Tasks.Add(new TaskItem { Title = "Update Budget" });
            Tasks.Add(new TaskItem { Title = "Update Goals" });
        }



        private async void OnBudgetClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///BudgetPage");
        }
        private async void OnGoalsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///GoalsPage");
        }
    }
}
