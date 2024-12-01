using System.Collections;
using UnityEngine;

public class NovelAIManager : MonoBehaviour
{
    private TokenManager tokenManager = new TokenManager();
    private NovelAPIRequest novelApiRequest;

    void Start()
    {
        // Assuming NovelAPIRequest is on the same GameObject
        novelApiRequest = GetComponent<NovelAPIRequest>();
    }

    public void SendMessageToAI(string userMessage)
    {
        // Add user message to history
        tokenManager.AddToHistory(userMessage + "\n");

        // Prepare the conversation history for the prompt
        string prompt = tokenManager.historyStr;

        // Send to NovelAI
        StartCoroutine(SendToNovelAI(prompt));
    }

    private IEnumerator SendToNovelAI(string prompt)
    {
        yield return StartCoroutine(novelApiRequest.RequestNovelAI(prompt));

        // Get and display AI response
        string aiResponse = novelApiRequest.GetLastResponse();
        if (!string.IsNullOrEmpty(aiResponse))
        {
            tokenManager.AddToHistory(aiResponse);
            // Update your UI to show the response
        }
    }
}
