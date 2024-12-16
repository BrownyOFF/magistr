using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ImageSendRequest : MonoBehaviour
{
    public SendRequest sendRequest;
    public Image image;
    
    private string API_URL = "https://api.openai.com/v1/images/generations";

    private void Start()
    {
        sendRequest = GetComponent<SendRequest>();
    }
    
    public IEnumerator GenerateImage(string prompt)
    {
        if (sendRequest.conversationHistory.Count == 0)
            yield break;
        if (sendRequest.isRequestInProgress)
        {
            Debug.Log("Запит вже обробляється.");
            yield break; // Виходимо з методу, якщо запит вже обробляється
        }
        sendRequest.isRequestInProgress = true;
        sendRequest.ShowPanelForLog("Generating Image...", false);
        var jsonData = JsonConvert.SerializeObject(new { model = PlayerPrefs.GetString("imageModel"), prompt = prompt, n = 1, size = "1024x1024" });
        var request = new UnityWebRequest(API_URL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {sendRequest.apiKey}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<DALLEImageResponse>(request.downloadHandler.text);
            StartCoroutine(LoadImage(response.data[0].url));
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }

        sendRequest.isRequestInProgress = false;
        sendRequest.resultPanel.SetActive(false);
    }
    public string GenerateUniqueFileName()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileExtension = ".png";
        string uniqueFileName = $"{sendRequest.worldNameBase}_{timestamp}{fileExtension}";
        return uniqueFileName;
    }
    private IEnumerator LoadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)); // Змінюємо компонент Image
            byte[] imageBytes = texture.EncodeToPNG();
            string filePath = Path.Combine(Application.persistentDataPath, GenerateUniqueFileName());

            File.WriteAllBytes(filePath, imageBytes);
            Debug.Log("Image saved at: " + filePath);
        }
        else
        {
            Debug.LogError("Error loading image: " + request.error);
        }
    }
    public IEnumerator LoadImageToUI(string filePath)
    {
        string fileUrl = "file://" + filePath; // Додаємо протокол file:// для локальних файлів
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(fileUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                image.sprite = sprite; // Встановлюємо у компонент Image
            }
            else
            {
                Debug.LogError($"Помилка завантаження зображення: {request.error}");
            }
        }
    }
    
    [System.Serializable]
    public class DALLEImageResponse
    {
        public DALLEImageData[] data;
    }

    [System.Serializable]
    public class DALLEImageData
    {
        public string url;
    }
}
