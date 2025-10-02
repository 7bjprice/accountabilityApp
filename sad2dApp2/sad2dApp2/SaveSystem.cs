using System;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sad2dApp2
{
    internal static class SaveSystem
    {
        public static bool SaveAcountagotchiToFile(string name, AcountaGotchi acountaGotchi)
        {
            try
            {
                string folderPath = Path.Combine(FileSystem.AppDataDirectory, "saveData");
                Directory.CreateDirectory(folderPath); // Ensure the directory exists
                string json = JsonSerializer.Serialize(acountaGotchi);
                string filePath = Path.Combine(folderPath, $"{name}_acountagotchi.json");
                File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving AcountaGotchi: {ex.Message}");
                return false;
            }
        }

        public static List<string> GetAllAcountaGotchiNames()
        {
            string folderPath = Path.Combine(FileSystem.AppDataDirectory, "saveData");
            if (!Directory.Exists(folderPath))
            {
                return new List<string>();
            }
            var files = Directory.GetFiles(folderPath, "*_acountagotchi.json");
            return files.Select(file => Path.GetFileNameWithoutExtension(file).Replace("_acountagotchi", "")).ToList();
        }

        public static AcountaGotchi LoadAcountagotchi(string name)
        {

            try
            {
                string folderPath = Path.Combine(FileSystem.AppDataDirectory, "saveData");
                string filePath = Path.Combine(folderPath, $"{name}_acountagotchi.json");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"AcountaGotchi file not found: {filePath}");
                    return null;
                }
                else
                {
                    string json = File.ReadAllText(filePath);
                    var acountaGotchi = JsonSerializer.Deserialize<AcountaGotchi>(json);
                    acountaGotchi?.UpdateStatsAfterLoad();
                    return acountaGotchi;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading AcountaGotchi: {ex.Message}");
                return null;
            }
        }
    }
}
