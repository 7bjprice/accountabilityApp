using System;
using System.Text.Json;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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

                await File.WriteAllTextAsync(filePath, json); // await here

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
    }
}
