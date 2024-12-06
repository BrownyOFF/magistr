using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;


public class SendRequest : MonoBehaviour
{
    // Memory to store the conversation history
    private List<Dictionary<string, string>> conversationHistory = new List<Dictionary<string, string>>();
    private string apiUrl;
    private string apiKey;
    private ShowMessage showMessage;
    public GameObject createPanel;

    void Start()
    {
        apiKey = PlayerPrefs.GetString("API");
        apiUrl = "https://api.openai.com/v1/chat/completions";
        showMessage = GetComponent<ShowMessage>();
    }

    // First request to start the story.
    public IEnumerator CreateStartingStory(string worldName, string characterName, string genre, string preferences) 
    {
        // createPanel.SetActive(false);
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

        yield return SendAPIRequest(apiUrl, apiKey);
    }

    /// Sends the API request and updates the conversation history with the response.
private IEnumerator SendAPIRequest(string apiUrl, string apiKey)
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
    /// Clears the conversation history (optional for starting fresh).
    public void ClearMemory()
    {
        conversationHistory.Clear();
        Debug.Log("Conversation history cleared.");
    }
}
