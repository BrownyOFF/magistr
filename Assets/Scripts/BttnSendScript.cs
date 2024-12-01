using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BttnSendScript : MonoBehaviour
{
    public GameObject textInput; // The InputField object
    private Button bttn; // The Button object
    public string userMessage;
    public TMP_Text storyText; // Reference to the TMP_Text component in the ScrollView's Content

    // Reference to your NovelAPIRequest script
    private NovelAPIRequest novelApiRequest;

    void Start()
    {
        // Get reference to the Button component
        bttn = this.gameObject.GetComponent<Button>();
        bttn.onClick.AddListener(ScriptSend);

        // Get the NovelAPIRequest component (make sure it's on the same GameObject or assign it in the Editor)
        novelApiRequest = GetComponent<NovelAPIRequest>();
    }

    void ScriptSend()
    {
        // Capture user input
        userMessage = textInput.GetComponent<TMP_InputField>().text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            Debug.Log("User message: " + userMessage);

            // Display user's message in the ScrollView
            AppendTextToStory(userMessage);

            // Send the request to NovelAI
            StartCoroutine(SendUserMessageToNovelAI(userMessage));
        }
    }

    IEnumerator SendUserMessageToNovelAI(string prompt)
    {
        // Send the prompt to NovelAI using the NovelAPIRequest script
        yield return StartCoroutine(novelApiRequest.RequestNovelAI(prompt));

        // Get the response
        string aiResponse = novelApiRequest.GetLastResponse();

        // Display AI's response in the ScrollView
        if (!string.IsNullOrEmpty(aiResponse))
        {
            AppendTextToStory(aiResponse); // Directly append AI response to the story
        }
    }

    // Append text to the story in the ScrollView
    private void AppendTextToStory(string newText)
    {
        Debug.Log("Appending text: " + newText); // Check if this logs in the Console
        storyText.text += "\n" + newText; // Update the story text
    }
}
