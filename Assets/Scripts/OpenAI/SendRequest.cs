using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SendRequest : MonoBehaviour
{
    // Memory to store the conversation history
    public List<Dictionary<string, string>> conversationHistory = new List<Dictionary<string, string>>();
    public string apiUrl;
    public string apiKey;
    private ShowMessage showMessage;
    private ImageSendRequest imageSendRequest;
    public GameObject createPanel;
    private TokenCount tokenCount;
    private SaveLoadScript saveLoadScript;
    public int maxTokens = 14000;
    public string worldNameBase; // Base name to access the saved file

    void Start()
    {
        apiKey = PlayerPrefs.GetString("API");
        apiUrl = "https://api.openai.com/v1/chat/completions";
        showMessage = GetComponent<ShowMessage>();
        tokenCount = GetComponent<TokenCount>();
        saveLoadScript = GetComponent<SaveLoadScript>();
        imageSendRequest = GetComponent<ImageSendRequest>();
    }

    // First request to start the story.
    public IEnumerator CreateStartingStory(string worldName, string characterName, string genre, string preferences) 
    {
        worldNameBase = worldName; // Save the world name to a separate variable for later access
        ClearMemory(); // Clear history if this is the first request
        conversationHistory.Add(new Dictionary<string, string> {
            { "role", "system" },
            { "content", "You are a creative story generator who writes immersive stories based on user input." }
        });

        // Add the user's input for the first story
        conversationHistory.Add(new Dictionary<string, string> {
            { "role", "user" },
            { "content", $"World Name: {worldName}. Character Name: {characterName}. Genre: {genre}. Preferences: {preferences}." }
        });

        // Save the conversation history to a file
        saveLoadScript.SaveConversationHistoryToFile(conversationHistory, worldName);

        yield return SendAPIRequest(apiUrl, apiKey);
    }

    // Subsequent requests to continue the story.
    public IEnumerator ContinueStory(string userInput) 
    {
        // display message
        Debug.Log("Continue story started");
        showMessage.AddUserMessage(userInput);

        // Add the user's input to the conversation history
        conversationHistory.Add(new Dictionary<string, string> {
            { "role", "user" },
            { "content", userInput }
        });
        
        if (tokenCount.CountTokensInConversation(conversationHistory) > maxTokens)
        {
            tokenCount.TrimConversationHistory(conversationHistory, maxTokens);
        }

        // Save the conversation history to a file
        saveLoadScript.SaveConversationHistoryToFile(conversationHistory, worldNameBase);
        
        yield return SendAPIRequest(apiUrl, apiKey);
    }

    // Load conversation history from a file
    public void LoadConversationHistory(string filePath)
    {
        conversationHistory = saveLoadScript.LoadConversationHistoryFromFile(filePath);
        worldNameBase = Path.GetFileNameWithoutExtension(filePath); // Extract world name from file path
        Debug.Log("Conversation history loaded.");
    }

    public int GetIndexFromConversationHistory(string content)
    {
        for (int i = 0; i < conversationHistory.Count; i++)
        {
            if (conversationHistory[i]["content"] == content)
            {
                return i; // Повертаємо індекс
            }
        }
        return -1; // Якщо не знайдено
    }

    public IEnumerator GeneratePromptForImage()
    {
        var conversationForImage = new List<Dictionary<string, string>>(conversationHistory);

        conversationForImage.Add(new Dictionary<string, string>
        {
            { "role", "user" },
            {
                "content",
                "Based on the current story in the conversation history, generate a detailed and creative prompt for the DALL·E model to create an image that reflects the current state of the narrative. The image should fit the tone and genre of the story (e.g., fantasy, sci-fi, or historical) and be visually compelling for use in an interactive story."
            }
        });

        var jsonData = new
        {
            model = "gpt-4o-mini",
            messages = conversationForImage,
            max_tokens = 250,
            temperature = 1
        };

        string jsonString = JsonConvert.SerializeObject(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonString);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("API Response: " + responseText);

                try
                {
                    JObject responseObject = JObject.Parse(responseText);
                    string message = responseObject["choices"]?[0]?["message"]?["content"]?.ToString();
                    if (!string.IsNullOrEmpty(message))
                    {
                        Debug.Log("Generated Prompt: " + message);
                        StartCoroutine(imageSendRequest.GenerateImage(message));
                    }
                    else
                    {
                        Debug.LogWarning("Response does not contain a valid prompt.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error parsing API response: " + e.Message);
                }
            }
            else
            {
                Debug.LogError($"Request failed with error: {request.error}. Response: {request.downloadHandler.text}");
            }
        }
    }

    public IEnumerator SendAPIRequest(string apiUrl, string apiKey)
    {
        var jsonData = new {
            model = "gpt-4o-mini",
            messages = conversationHistory,
            max_tokens = PlayerPrefs.GetInt("maxTok"),
            temperature = PlayerPrefs.GetFloat("temp")
        };

        string jsonString = JsonConvert.SerializeObject(jsonData);
        Debug.Log("Request Payload: " + jsonString);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonString);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("API Response: " + responseText); // Print full response to inspect

                try
                {
                    // Parse the response into JObject
                    JObject responseObject = JObject.Parse(responseText);

                    // Check if choices exists and is not empty
                    if (responseObject["choices"] != null && responseObject["choices"].HasValues)
                    {
                        string message = responseObject["choices"][0]["message"]["content"].ToString();

                        // Add the response to conversation history
                        conversationHistory.Add(new Dictionary<string, string>
                        {
                            { "role", "assistant" },
                            { "content", message }
                        });

                        // Display the response
                        showMessage.AddAIMessage(message);
                        createPanel.SetActive(false);
                        
                        // save
                        saveLoadScript.SaveConversationHistoryToFile(conversationHistory, worldNameBase);
                    }
                    else
                    {
                        Debug.LogError("Choices array is empty or null.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Error parsing the response: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError($"Request Failed: {request.error}\nResponse: {request.downloadHandler.text}");
            }
        }
    }

    // Clears the conversation history (optional for starting fresh).
    public void ClearMemory()
    {
        conversationHistory.Clear();
        Debug.Log("Conversation history cleared.");
    }
}
