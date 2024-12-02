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

    void Start()
    {
        AddUserMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur et justo metus. Ut volutpat posuere nisi, sed sodales dui. Aliquam efficitur metus quam, eu lacinia ex imperdiet id. Vivamus bibendum ex accumsan vehicula tristique. Vestibulum nec tincidunt magna, vitae aliquam velit. Maecenas in molestie neque, eleifend auctor metus. Quisque dapibus, nisi vitae aliquam mattis, mauris purus bibendum libero, sed egestas lorem purus et ante. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.");
        AddAIMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur et justo metus. Ut volutpat posuere nisi, sed sodales dui. Aliquam efficitur metus quam, eu lacinia ex imperdiet id. Vivamus bibendum ex accumsan vehicula tristique. Vestibulum nec tincidunt magna, vitae aliquam velit. Maecenas in molestie neque, eleifend auctor metus. Quisque dapibus, nisi vitae aliquam mattis, mauris purus bibendum libero, sed egestas lorem purus et ante. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.");
    }
}
