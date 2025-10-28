using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace sad2dApp2
{
    internal static class SaveSystem
    {
        public static async Task<bool> SaveAcountagotchiToFileAsync(string name, AcountaGotchi acountaGotchi)
        {
            try
            {
                string folderPath = Path.Combine(FileSystem.AppDataDirectory, "saveData");
                Directory.CreateDirectory(folderPath); // Ensure the directory exists

                string json = JsonSerializer.Serialize(acountaGotchi);
                string filePath = Path.Combine(folderPath, $"{name}_acountagotchi.json");

                await File.WriteAllTextAsync(filePath, json);

                Debug.WriteLine($"Saved AcountaGotchi to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving AcountaGotchi: {ex.Message}");
                return false;
            }
        }

        public static async Task<List<string>> GetAllAcountaGotchiNamesAsync()
        {
            string folderPath = Path.Combine(FileSystem.AppDataDirectory, "saveData");
            if (!Directory.Exists(folderPath))
            {
                return new List<string>();
            }

            return await Task.Run(() =>
            {
                var files = Directory.GetFiles(folderPath, "*_acountagotchi.json");
                return files.Select(file =>
                    Path.GetFileNameWithoutExtension(file).Replace("_acountagotchi", "")
                ).ToList();
            });
        }

        public static async Task<AcountaGotchi?> LoadAcountagotchiAsync(string name)
        {
            try
            {
                string folderPath = Path.Combine(FileSystem.AppDataDirectory, "saveData");
                string filePath = Path.Combine(folderPath, $"{name}_acountagotchi.json");

                if (!File.Exists(filePath))
                {
                    Debug.WriteLine($"AcountaGotchi file not found: {filePath}");
                    return null;
                }

                string json = await File.ReadAllTextAsync(filePath);
                var acountaGotchi = JsonSerializer.Deserialize<AcountaGotchi>(json);
                acountaGotchi?.UpdateStatsAfterLoad();
                return acountaGotchi;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading AcountaGotchi: {ex.Message}");
                return null;
            }
        }

        public static async Task<bool> SaveBudgetItems(ObservableCollection<BudgetItem> items, double totalBudget)
        {
            try
            {
                string folderPath = Path.Combine(FileSystem.AppDataDirectory, "budget");
                Directory.CreateDirectory(folderPath); // Ensure the directory exists

                string json = JsonSerializer.Serialize(
                    new { TotalBudget = totalBudget, Items = items },
                    new JsonSerializerOptions { WriteIndented = true }
                );
                string filePath = Path.Combine(folderPath, $"budget.json");

                await File.WriteAllTextAsync(filePath, json);
                Debug.WriteLine($"Saved budget items to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving budget items: {ex.Message}");
                return false;
            }
        }

        public static async Task<(ObservableCollection<BudgetItem>, double)> LoadBudgetItems()
        {
            try
            {
                string folderPath = Path.Combine(FileSystem.AppDataDirectory, "budget");
                string filePath = Path.Combine(folderPath, "budget.json");

                if (!File.Exists(filePath))
                {
                    Debug.WriteLine("Budget file not found, returning empty collection.");
                    return (new ObservableCollection<BudgetItem>(), 0);
                }

                string json = await File.ReadAllTextAsync(filePath);

                // Define a helper type to match the saved JSON structure
                var data = JsonSerializer.Deserialize<BudgetData>(json);

                var items = data?.Items != null
                    ? new ObservableCollection<BudgetItem>(data.Items)
                    : new ObservableCollection<BudgetItem>();

                double totalBudget = data?.TotalBudget ?? 0;

                Debug.WriteLine($"Loaded {items.Count} budget items with total budget {totalBudget}");
                return (items, totalBudget);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading budget items: {ex.Message}");
                return (new ObservableCollection<BudgetItem>(), 0);
            }
        }
        class BudgetData
        {
            public double TotalBudget { get; set; }
            public List<BudgetItem> Items { get; set; }
        }


        public static async Task<bool> SaveGoalsItems(ObservableCollection<GoalsItem> items, double totalGoals)
        {
            try
            {
                string folderPath = Path.Combine(FileSystem.AppDataDirectory, "goals");
                Directory.CreateDirectory(folderPath); // Ensure the directory exists

                // Wrap items and totalGoals in GoalsData
                GoalsData data = new GoalsData
                {
                    TotalGoals = totalGoals,
                    Items = items.ToList()
                };

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                string filePath = Path.Combine(folderPath, $"goals.json");

                await File.WriteAllTextAsync(filePath, json);
                Debug.WriteLine($"Saved goals items to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving goals items: {ex.Message}");
                return false;
            }
        }

        public static async Task<(ObservableCollection<GoalsItem>, double)> LoadGoalsItems()
        {
            try
            {
                string folderPath = Path.Combine(FileSystem.AppDataDirectory, "goals");
                string filePath = Path.Combine(folderPath, "goals.json");

                if (!File.Exists(filePath))
                {
                    Debug.WriteLine("Goals file not found, returning empty collection.");
                    return (new ObservableCollection<GoalsItem>(), 0);
                }

                string json = await File.ReadAllTextAsync(filePath);

                // Deserialize into GoalsData
                var data = JsonSerializer.Deserialize<GoalsData>(json);

                var items = data?.Items != null
                    ? new ObservableCollection<GoalsItem>(data.Items)
                    : new ObservableCollection<GoalsItem>();

                double totalGoals = data?.TotalGoals ?? 0;

                Debug.WriteLine($"Loaded {items.Count} goals items with total goals {totalGoals}");
                return (items, totalGoals);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading goals items: {ex.Message}");
                return (new ObservableCollection<GoalsItem>(), 0);
            }
        }

        // Helper class for JSON serialization
        class GoalsData
        {
            public double TotalGoals { get; set; }
            public List<GoalsItem> Items { get; set; } = new();
        }

        public static async Task<bool> DeleteBudgetFileAsync()
        {
            try
            {
                string folderPath = Path.Combine(FileSystem.AppDataDirectory, "budget");
                string filePath = Path.Combine(folderPath, "budget.json");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.WriteLine($"Budget file deleted: {filePath}");
                }
                else
                {
                    Debug.WriteLine("Budget file does not exist, nothing to delete.");
                }

                // Since the operation is synchronous, wrap in a completed Task
                await Task.CompletedTask;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting budget file: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> DeleteAllSaveFilesAsync()
        {
            try
            {
                // Define all relevant save folders
                string appData = FileSystem.AppDataDirectory;
                string[] folders = { "saveData", "budget", "goals" };

                foreach (var folderName in folders)
                {
                    string folderPath = Path.Combine(appData, folderName);

                    if (Directory.Exists(folderPath))
                    {
                        // Delete all files in the folder
                        foreach (var file in Directory.GetFiles(folderPath))
                        {
                            File.Delete(file);
                            Debug.WriteLine($"Deleted file: {file}");
                        }

                        // Optionally delete the folder itself
                        Directory.Delete(folderPath, false); // false = only if empty
                        Debug.WriteLine($"Deleted folder: {folderPath}");
                    }
                    else
                    {
                        Debug.WriteLine($"Folder does not exist: {folderPath}");
                    }
                }

                await Task.CompletedTask;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting save files: {ex.Message}");
                return false;
            }
        }


    }
}
