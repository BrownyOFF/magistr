using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageGenerateBttn : MonoBehaviour
{
    Button bttn;
    private SendRequest sendRequest;

    void Start()
    {
        bttn = gameObject.GetComponent<Button>();
        bttn.onClick.AddListener(Script);
        sendRequest = GameObject.FindWithTag("Manager").GetComponent<SendRequest>();
    }

    // Update is called once per frame
    void Script()
    {
        if (sendRequest.conversationHistory.Count == 0)
        {
            Debug.Log("No conversation found");
            return;
        }
        StartCoroutine(sendRequest.GeneratePromptForImage());
    }
}
