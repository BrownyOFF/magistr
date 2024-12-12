using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MessageScript : MonoBehaviour, IPointerClickHandler
{
    public string messageText = "";
    public int id;
    public TMP_Text messageBox;
    public TMP_InputField inputField;
    public SendRequest sendRequest;
    public SaveLoadScript saveLoadScript;

    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;

    void Start()
    {
        inputField = transform.GetChild(1).GetComponent<TMP_InputField>();
        sendRequest = GameObject.FindWithTag("Manager").GetComponent<SendRequest>();
        saveLoadScript = GameObject.FindWithTag("Manager").GetComponent<SaveLoadScript>();
    }

    public void Set(int idc, string message)
    {
        messageText = message;
        id = idc;
        inputField.gameObject.SetActive(false);
        messageBox.text = messageText;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            StartEditing();
            Debug.Log("clicked");
        }
        lastClickTime = Time.time;
    }

    private void StartEditing()
    {
        inputField.text = messageText;
        inputField.gameObject.SetActive(true);
        messageBox.gameObject.SetActive(false);
        inputField.ActivateInputField(); // Фокус на InputField
    }

    void Update()
    {
        if (inputField.isFocused)
        {
            // Перевірка на натискання Enter або Shift + Enter
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    AddLineBreak(); // Додаємо абзац
                }
                else
                {
                    SaveText(); // Зберігаємо текст
                }
            }

            // Перевірка на натискання ESC для скасування
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelEditing(); // Відміна редагування
            }
        }
    }

    private void AddLineBreak()
    {
        // Додаємо абзац в поточне місце курсора
        int cursorPosition = inputField.caretPosition;
        inputField.text = inputField.text.Insert(cursorPosition, "\n");
        inputField.caretPosition = cursorPosition + 1; // Оновлюємо позицію курсора
    }

    public void SaveText()
    {
        messageBox.text = inputField.text;
        messageText = inputField.text;
        inputField.gameObject.SetActive(false);
        messageBox.gameObject.SetActive(true);
        sendRequest.conversationHistory[id]["content"] = messageText;
        saveLoadScript.SaveConversationHistoryToFile(sendRequest.conversationHistory, sendRequest.worldName);
    }

    private void CancelEditing()
    {
        inputField.text = messageText; // Повертаємо старий текст
        inputField.gameObject.SetActive(false);
        messageBox.gameObject.SetActive(true);
    }
}