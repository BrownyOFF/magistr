using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateWorld : MonoBehaviour
{
    public string nameWorld;
    public string nameChara;
    public string Genre;
    public string Preferenses;

    public TMP_InputField world;
    public TMP_InputField chara;
    public TMP_InputField genre;
    public TMP_InputField preferenses;

    private Button bttn;

    private SendRequest send;
    private ShowMessage showMessage;
    public GameObject wholePanel;

    void Start()
    {
        bttn = gameObject.GetComponent<Button>();
        bttn.onClick.AddListener(generate);
        send = GameObject.FindWithTag("Manager").GetComponent<SendRequest>();
        showMessage = GameObject.FindWithTag("Manager").GetComponent<ShowMessage>();
    }

    public void generate()
    {
        nameWorld = world.text.ToString();
        nameChara = chara.text.ToString();
        Genre = genre.text.ToString();
        Preferenses = preferenses.text.ToString();
        Debug.Log("nameWorld:" + nameWorld + "\n" +"nameChara:" + nameChara + "\n" +"Genre:" + Genre + "\n" +"Preferenses:" + Preferenses);
        StartCoroutine(send.CreateStartingStory(nameWorld, nameChara, Genre, Preferenses));
    }

     public void DisplayAIResponse(string aiMessage)
    {
        showMessage.AddAIMessage(aiMessage);  // Display the AI's message (story start)
        wholePanel.SetActive(false);
    }
}
