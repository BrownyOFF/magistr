using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;

public class NovelAPIRequest : MonoBehaviour
{
    private string apiUrl = "https://api.novelai.net/ai/generate"; // Corrected URL
    private string apiKey = "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"; // Your API key

    public TokenManager tokenManager;
    // private List<string> conversationHistory = new List<string>();

    private string lastResponse;

    public void Start()
    {
        tokenManager = GetComponent<TokenManager>();
    }
    public IEnumerator RequestNovelAI(string prompt)
    {
    string fullPrompt = prompt;

    // Create a JSON body with the full conversation history
    string jsonData = "{\"input\": \"" + fullPrompt + "\", \"model\": \"euterpe-v2\", \"parameters\": {\"use_string\": true, \"temperature\": 0.7, \"min_length\": 10, \"max_length\": 1024}}";

    // Prepare the UnityWebRequest
    UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();
    
    // Set headers as per the `curl` example
    request.SetRequestHeader("accept", "application/json");
    request.SetRequestHeader("Content-Type", "application/json");
    request.SetRequestHeader("Authorization", "Bearer " + apiKey); // Ensure your API key is correct

    // Send the request and wait for a response
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {   
        Debug.Log("Response: " + request.downloadHandler.text);
        
        // Parse the JSON response to get the actual output text
        AIResponse response = JsonUtility.FromJson<AIResponse>(request.downloadHandler.text);
        string aiResponse = response.output;

        // Add the AI's response to the conversation history
        tokenManager.AddToHistory(aiResponse);
        //conversationHistory.Add("AI: " + aiResponse);

        // Store the response
        lastResponse = aiResponse;
    }
    else
    {
        Debug.LogError("Request failed: " + request.error);
        Debug.LogError("Response Code: " + request.responseCode);
        Debug.LogError("Response Body: " + request.downloadHandler.text);
    }   
}

    public string GetLastResponse()
    {
        return lastResponse;
    }


    [System.Serializable]
    public class AIResponse
    {
        public string output;
    }
}
