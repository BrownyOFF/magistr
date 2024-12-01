using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AI21APIRequest : MonoBehaviour
{
    private string apiUrl = "https://api.novelai.net/ai/generate"; // Corrected URL
    private string apiKey = "pst-gbRyejzZjqq1wjqQb9qnUWtDAsJ2yZuJ6QZRjX2MmsX4gaKrWPYYL5XdX2mMVstJ"; // Your API key

    void Start()
    {
        // Start the request to NovelAI with a sample prompt
        StartCoroutine(RequestNovelAI("Text generation example."));
    }

    IEnumerator RequestNovelAI(string prompt)
    {
        // Create a JSON body as per the `curl` request format
        string jsonData = "{\"input\": \"" + prompt + "\", \"model\": \"euterpe-v2\", \"parameters\": {\"use_string\": true, \"temperature\": 1, \"min_length\": 10, \"max_length\": 30}}";

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

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            // Process the response here, e.g., parse the generated text
        }
        else
        {
            // Log the error response for debugging
            Debug.LogError("Request failed: " + request.error);
            Debug.LogError("Response Code: " + request.responseCode);
            Debug.LogError("Response Body: " + request.downloadHandler.text);
        }
    }
}