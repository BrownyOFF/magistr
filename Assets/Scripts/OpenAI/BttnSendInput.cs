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

    void Start()
    {
        send = GameObject.FindWithTag("Manager").GetComponent<SendRequest>();
        bttn = gameObject.GetComponent<Button>();
        bttn.onClick.AddListener(SendInput);
    }

    // Update is called once per frame
    void SendInput()
    {
        userInput = inputField.GetComponent<TMP_InputField>().text;
        send.ContinueStory(userInput);
    }
}
