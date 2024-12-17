using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine.UIElements;

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
    public int maxTokens = 125000;
    public string worldNameBase; // Base name to access the saved file
    public int count;
    public GameObject resultPanel;
    public bool isRequestInProgress = false;
    public TMP_InputField inputField;
    
    public void ShowPanelForLog(string message, bool needButton)
    {
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(1).gameObject.SetActive(false);
        resultPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = message;
        if(needButton)
            resultPanel.transform.GetChild(1).gameObject.SetActive(true);
    }
    
    void Start()
    {
        apiKey = PlayerPrefs.GetString("API");
        apiUrl = "https://api.openai.com/v1/chat/completions";
        showMessage = GetComponent<ShowMessage>();
        tokenCount = GetComponent<TokenCount>();
        saveLoadScript = GetComponent<SaveLoadScript>();
        imageSendRequest = GetComponent<ImageSendRequest>();
        count = 0;
    }

    // First request to start the story.
    public IEnumerator CreateStartingStory(string worldName, string characterName, string genre, string preferences) 
    {
        worldNameBase = worldName; // Save the world name to a separate variable for later access
        
        ClearMemory(); 
        if(showMessage.messagesObjects.Count != 0)
            showMessage.DeleteAllMessages();// Clear history if this is the first request
        
        var startMessage = "";
        if (PlayerPrefs.GetInt("familyMode") == 1)
        {
            startMessage =
                "You are a story generator designed for family-friendly content. Your role is to create stories suitable for children, ensuring they are engaging, positive, and appropriate for younger audiences. Avoid any form of violence, cruelty, complex romantic themes, profanity, or content that could be deemed unsuitable for families. Focus instead on uplifting adventures, educational elements, humor, and messages that encourage kindness, creativity, teamwork, and imagination.\n\nStructure your responses as a game-like narrative, speaking directly to the player in the second person. Begin each scene with a vivid, child-friendly description of the environment, characters, and events. Always provide the player with clear, exciting choices for their next actions and never make decisions on their behalf. Emphasize positive consequences and playful exploration while inspiring curiosity and problem-solving.\n\nEnsure your responses are concise and respect the token limit. If necessary, provide a complete and logical conclusion using fewer tokens without compromising the story's coherence or charm. Adjust the tone to remain lighthearted, whimsical, or adventurous, depending on the genre or setting. In cases where a more detailed response would exceed the token limit, focus on delivering a cohesive and delightful segment and let the player know the story will continue based on their next input.\n";
        }
        else
        {
            startMessage = "You are a creative and engaging story generator. Your role is to narrate immersive, branching stories in the second person, directly addressing the player. Structure your responses as a game-like narrative, beginning each scene with a vivid description of the environment, characters, and events. Always conclude your responses by offering the player clear choices for their next action, without making decisions for them. Your narration should reflect the consequences of their past actions and guide them through the world with intrigue and emotional depth.\n\nEnsure your responses are concise and avoid exceeding the token limit. If necessary, provide a complete and logical conclusion using fewer tokens while maintaining narrative quality. In cases where a detailed response is required but tokens are limited, prioritize delivering a cohesive and complete segment, and indicate that the story will continue based on the player's next input. Adjust the tone and style to match the genre (e.g., fantasy, sci-fi, or adventure) while immersing the player in the unfolding events.\n";
        }
        conversationHistory.Add(new Dictionary<string, string> {
            { "role", "system" },
            { "content", startMessage }
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
        if (isRequestInProgress)
        {
            Debug.Log("Запит вже обробляється.");
            yield break; // Виходимо з методу, якщо запит вже обробляється
        }
        // display message
        Debug.Log("Continue story started");
        if(userInput != "")
        {
            showMessage.AddUserMessage(userInput);
        }

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
        ShowPanelForLog("Loading prompt for image...", false);
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
            model = PlayerPrefs.GetString("textModel"),
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
            request.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("API")}");
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
                        ShowPanelForLog("Response does not contain a valid prompt.", true);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error parsing API response: " + e.Message);
                    ShowPanelForLog($"Error parsing the response: {e.Message}", true);
                }
            }
            else
            {
                Debug.LogError($"Request failed with error: {request.error}. Response: {request.downloadHandler.text}");
                ShowPanelForLog($"Error parsing the response: {request.error}", true);
            }
        }
    }

    public IEnumerator SendAPIRequest(string apiUrl, string apiKey)
    {
        if (isRequestInProgress)
        {
            Debug.Log("Запит вже обробляється.");
            yield break; // Виходимо з методу, якщо запит вже обробляється
        }

        isRequestInProgress = true; // Встановлюємо, що запит почав оброблятися
        ShowPanelForLog("Loading prompt to server...", false);
        
        var jsonData = new {
            model = PlayerPrefs.GetString("textModel"),
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
            request.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("API")}");
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
                        DeleteMessage("Choices array is empty or null.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Error parsing the response: " + ex.Message);
                    DeleteMessage(ex.Message);
                    
                }
            }
            else
            {
                Debug.LogError($"Request Failed: {request.error}\nResponse: {request.downloadHandler.text}");
                DeleteMessage(request.error);
            }
            isRequestInProgress = false; // Встановлюємо, що запит завершено
            resultPanel.SetActive(false);
        }
    }

    void DeleteMessage(string message)
    {
        ShowPanelForLog($"Error parsing the response: {message}", true);
                    
        var messageTemp = conversationHistory[conversationHistory.Count - 1];
        string tmpMessage = messageTemp["content"];
        inputField.text = tmpMessage;
                    
        Destroy(showMessage.messagesObjects[showMessage.messagesObjects.Count - 1]);
        showMessage.messagesObjects.RemoveAt(showMessage.messagesObjects.Count - 1);
        conversationHistory.RemoveAt(conversationHistory.Count - 1);
    }
    // Clears the conversation history (optional for starting fresh).
    public void ClearMemory()
    {
        conversationHistory.Clear();
        Debug.Log("Conversation history cleared.");
    }
}
