using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ShowMessage : MonoBehaviour
{
    public Transform content; // Reference to the Content object
    public GameObject userMessagePrefab; // Prefab for user messages
    public GameObject aiMessagePrefab; // Prefab for AI messages
    public SendRequest sendRequest;

    public List<GameObject> messagesObjects = new List<GameObject>();
    
    public void Start()
    {
        sendRequest = GameObject.FindWithTag("Manager").GetComponent<SendRequest>();
    }

    // Function to add a user message
    public void AddUserMessage(string message)
    {
        GameObject newMessage = Instantiate(userMessagePrefab, content);
        newMessage.GetComponent<MessageScript>().messageText = message;
        
        messagesObjects.Add(newMessage);
    }

    // Function to add an AI response
    public void AddAIMessage(string message)
    {
        GameObject newMessage = Instantiate(aiMessagePrefab, content);
        newMessage.GetComponent<MessageScript>().messageText = message;

        messagesObjects.Add(newMessage);
    }

    public void RegenerateMessage()
    {
        GameObject var = messagesObjects.Last();
        messagesObjects.RemoveAt(messagesObjects.Count - 1);
        sendRequest.conversationHistory.RemoveAt(sendRequest.conversationHistory.Count - 1);
        Destroy(var);
        StartCoroutine(sendRequest.SendAPIRequest(sendRequest.apiUrl, sendRequest.apiKey));
    }
}
