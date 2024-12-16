using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SaveLoadScript : MonoBehaviour
{
    public GameObject[] arr;
    public GameObject bttnPrefab;
    public Transform scrollContent;
    public ShowMessage showMessage;
    public GameObject loadPanel;
    public SendRequest sendRequest;
    public ImageSendRequest imageSendRequest;
    
    public void Start()
    {
        showMessage = gameObject.GetComponent<ShowMessage>();
        sendRequest = gameObject.GetComponent<SendRequest>();
        imageSendRequest = gameObject.GetComponent<ImageSendRequest>();
    }

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
    
    // Clean prefabs
    public void CleanBttn()
    {
        arr = GameObject.FindGameObjectsWithTag("loadbttn");
        if(arr.Length == 0)
            return;
        
        foreach (var i in arr)
        {
            Destroy(i);
        }
    }
    
    //load files and bttns
    public void ReadAndLoadBttns()
    {
        string savePath = Application.persistentDataPath;
        string[] files = Directory.GetFiles(savePath, "*.json");
        
        CleanBttn();
        foreach (string file in files)
        {
            string worldName = Path.GetFileNameWithoutExtension(file);
            
            GameObject button = Instantiate(bttnPrefab, scrollContent);
            button.GetComponentInChildren<TMP_Text>().text = worldName;

            button.GetComponent<Button>().onClick.AddListener(() => LoadWorld(file));
        }
    }
    
    private void LoadWorld(string filePath)
    {
        List<Dictionary<string, string>> conversationHistory = LoadConversationHistoryFromFile(filePath);
        string worldName = Path.GetFileNameWithoutExtension(filePath);
        Debug.Log($"World loaded: {worldName}");

        int i = 0;
        foreach (var message in conversationHistory)
        {
            string role = message["role"];
            string content = message["content"];
            
            if (role == "user")
            {
                showMessage.AddUserMessage(content);
            }
            else if (role == "assistant")
            {
                showMessage.AddAIMessage(content);
            }
            i++;
        }
        sendRequest.LoadConversationHistory(filePath);
        GetLatestImageFile();
        loadPanel.SetActive(false);
    }

    public void GetLatestImageFile()
    {
        string folderPath = Application.persistentDataPath;
        string fileExtension = ".png";

        string[] files = Directory.GetFiles(folderPath, $"*{fileExtension}");

        var filteredFiles = files.Where(file =>
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            return fileName.StartsWith(sendRequest.worldNameBase) && long.TryParse(fileName.Split('_').Last(), out _);
        });

        if (!filteredFiles.Any())
        {
            Debug.LogWarning("No image files found matching the format.");
            return;
        }

        string latestFile = filteredFiles
            .OrderByDescending(file =>
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                long timestamp = long.Parse(fileName.Split('_').Last());
                return timestamp;
            })
            .FirstOrDefault();

        // Виклик корутини
        StartCoroutine(imageSendRequest.LoadImageToUI(latestFile));
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