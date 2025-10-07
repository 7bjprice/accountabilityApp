﻿using CommunityToolkit.Maui;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace sad2dApp2
{
    public partial class MainPage : ContentPage
    {
        public AcountaGotchi? CurrentAcountaGotchi { get; set; }
        public ObservableCollection<TaskItem> Tasks { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();

            _ = InitializeGotchiAsync();
        }

        private async Task InitializeGotchiAsync()
        {
            var acountaGotchiNames = await SaveSystem.GetAllAcountaGotchiNamesAsync();
            Debug.WriteLine($"Found {acountaGotchiNames.Count} AcountaGotchi(s).");

            if (acountaGotchiNames.Count == 0)
            {
                // No AcountaGotchi found, create and save a new one
                var newAcountaGotchi = new AcountaGotchi("Default");
                CurrentAcountaGotchi = newAcountaGotchi;

                await SaveSystem.SaveAcountagotchiToFileAsync(newAcountaGotchi.Name, newAcountaGotchi);
                Debug.WriteLine($"Created and saved new AcountaGotchi: {newAcountaGotchi.Name}");
            }
            else
            {
                // Load the first AcountaGotchi
                CurrentAcountaGotchi = await SaveSystem.LoadAcountagotchiAsync(acountaGotchiNames[0]);
                Debug.WriteLine($"Loaded AcountaGotchi: {CurrentAcountaGotchi?.Name}");
            }

            LoadDailyTasks();
            TasksList.ItemsSource = Tasks;
        }

        private void LoadDailyTasks()
        {
            Tasks.Add(new TaskItem { Title = "Feed Umbreon" });
            Tasks.Add(new TaskItem { Title = "Give water" });
            Tasks.Add(new TaskItem { Title = "Take a walk" });
            Tasks.Add(new TaskItem { Title = "Play together" });
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
