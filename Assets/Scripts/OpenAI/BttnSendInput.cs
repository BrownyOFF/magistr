using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BttnSendInput : MonoBehaviour
{
    private Button bttn;
    private SendRequest send;
    public GameObject inputField;
    private string userInput;
    public bool isRegenerate;
    private ShowMessage showMessage;

    void Start()
    {
        send = GameObject.FindWithTag("Manager").GetComponent<SendRequest>();
        bttn = gameObject.GetComponent<Button>();
        showMessage = GameObject.FindWithTag("Manager").GetComponent<ShowMessage>();
        bttn.onClick.AddListener(SendInput);
    }

    // Update is called once per frame
    void SendInput()
    {
        Debug.Log("Bttn Pressed");
        if (send.isRequestInProgress || send.conversationHistory.Count == 0)
        {
            return;
        }
        if (isRegenerate)
        {
            showMessage.RegenerateMessage();
        }
        else
        {
            userInput = inputField.GetComponent<TMP_InputField>().text;
            inputField.GetComponent<TMP_InputField>().text = "";
            StartCoroutine(send.ContinueStory(userInput));
        }
    }
}
