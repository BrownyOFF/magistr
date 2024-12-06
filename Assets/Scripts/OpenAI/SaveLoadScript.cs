using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadScript : MonoBehaviour
{
    // Метод для збереження історії розмови у файл
    public void SaveConversationHistoryToFile(List<Dictionary<string, string>> conversationHistory, string worldName)
    {
        // Шлях до файлу зберігання
        string filePath = Path.Combine(Application.persistentDataPath, $"{worldName}.json");
        Debug.Log(Application.persistentDataPath);

        // Перевірка на існування файлу
        if (!File.Exists(filePath))
        {
            // Якщо файл не існує, створити новий файл
            File.Create(filePath).Dispose(); // Використовуємо Dispose для негайного звільнення ресурсу
        }

        // Серіалізація історії в JSON формат
        string jsonData = JsonConvert.SerializeObject(conversationHistory, Formatting.Indented);

        // Запис JSON даних у файл
        File.WriteAllText(filePath, jsonData);
        Debug.Log($"Conversation history saved to {filePath}");
    }

    // Метод для завантаження історії розмови з файлу
    public List<Dictionary<string, string>> LoadConversationHistoryFromFile(string filePath)
    {
        // Перевірка на існування файлу перед завантаженням
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonData);
        }
        else
        {
            // Якщо файл не існує, повернути порожній список
            Debug.LogWarning("File not found. Returning empty history.");
            return new List<Dictionary<string, string>>();
        }
    }
}