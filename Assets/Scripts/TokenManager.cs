using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TokenManager : MonoBehaviour
{
    public string historyStr = "";
    public string historyBackUp;
    private List<string> history = new List<string>(); // Keeps track of the conversation
    private const int maxTokens = 1024;

    // Adds a new message to the conversation history
    public void AddToHistory(string message)
    {
        historyBackUp = historyStr;
        historyStr += message;
    }

}
