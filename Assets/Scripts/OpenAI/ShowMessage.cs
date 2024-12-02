using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowMessage : MonoBehaviour
{
    public Transform content; // Reference to the Content object
    public GameObject userMessagePrefab; // Prefab for user messages
    public GameObject aiMessagePrefab; // Prefab for AI messages

    // Function to add a user message
    public void AddUserMessage(string message)
    {
        GameObject newMessage = Instantiate(userMessagePrefab, content);
        TMP_Text messageText = newMessage.transform.GetChild(0).GetComponent<TMP_Text>(); // Access the first child (Text TMP)
        messageText.text = message; // Set the text
    }

    // Function to add an AI response
    public void AddAIMessage(string message)
    {
        GameObject newMessage = Instantiate(aiMessagePrefab, content);
        TMP_Text messageText = newMessage.transform.GetChild(0).GetComponent<TMP_Text>(); // Access the first child (Text TMP)
        messageText.text = message; // Set the text
    }

}
