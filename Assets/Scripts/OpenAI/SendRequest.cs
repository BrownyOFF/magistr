using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SendRequest : MonoBehaviour
{
    public IEnumerator CreateStartingStory(string worldName, string characterName, string genre, string preferences) 
    {
        string apiUrl = "https://api.openai.com/v1/chat/completions";
        string apiKey = PlayerPrefs.GetString("API");

        // Construct the prompt based on user inputs
        var jsonData = new {
            model = "gpt-4o-mini",
            messages = new[] {
                new { role = "system", content = "You are a creative story generator who writes immersive stories based on user input." },
                new { role = "user", content = $"World Name: {worldName}. Character Name: {characterName}. Genre: {genre}. Preferences: {preferences}." }
            },
            max_tokens = PlayerPrefs.GetInt("maxTok"),
            temperature = PlayerPrefs.GetInt("temp")
        };
        Debug.Log(jsonData);

        // Serialize JSON data
        string jsonString = JsonConvert.SerializeObject(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")) {
            byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonString);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request and wait for the response
            yield return request.SendWebRequest();

            // Log response or errors
            if (request.result == UnityWebRequest.Result.Success) {
                string responseText = request.downloadHandler.text;
                Debug.Log("API Response: " + responseText);
            } else {
                Debug.LogError($"Request Failed: {request.error}\nResponse: {request.downloadHandler.text}");
            }
        }
    }
}
