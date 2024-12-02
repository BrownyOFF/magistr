using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SendRequest : MonoBehaviour
{
    // Memory to store the conversation history
    private List<Dictionary<string, string>> conversationHistory = new List<Dictionary<string, string>>();

    // First request to start the story.
    public IEnumerator CreateStartingStory(string worldName, string characterName, string genre, string preferences) 
    {
        string apiUrl = "https://api.openai.com/v1/chat/completions";
        string apiKey = PlayerPrefs.GetString("API");

        // Add the initial system message
        conversationHistory.Clear(); // Clear history if this is the first request
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
        string apiUrl = "https://api.openai.com/v1/chat/completions";
        string apiKey = PlayerPrefs.GetString("API");

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
        // Construct the request data
        var jsonData = new {
            model = "gpt-4o-mini",
            messages = conversationHistory,
            max_tokens = PlayerPrefs.GetInt("maxTok"),
            temperature = PlayerPrefs.GetFloat("temp")
        };

        // Serialize JSON data
        string jsonString = JsonConvert.SerializeObject(jsonData);
        Debug.Log("Request Payload: " + jsonString);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")) {
            byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonString);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request and wait for the response
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string responseText = request.downloadHandler.text;
                Debug.Log("API Response: " + responseText);

                // Parse the assistant's response and add it to the conversation history
                var responseObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);
                if (responseObject != null && responseObject.ContainsKey("choices"))
                {
                    var choices = responseObject["choices"] as IList<object>;
                    if (choices != null && choices.Count > 0)
                    {
                        var choice = choices[0] as Dictionary<string, object>;
                        if (choice != null && choice.ContainsKey("message"))
                        {
                            var assistantMessage = choice["message"] as Dictionary<string, string>;
                            if (assistantMessage != null)
                            {
                                conversationHistory.Add(new Dictionary<string, string> {
                                    { "role", "assistant" },
                                    { "content", assistantMessage["content"] }
                                });
                                Debug.Log("Assistant Response Saved to History: " + assistantMessage["content"]);
                            }
                        }
                    }
                }
            } else {
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
