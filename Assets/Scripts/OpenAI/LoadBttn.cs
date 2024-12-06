using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadBttn : MonoBehaviour
{
    private Button bttn;
    private SaveLoadScript saveLoadScript;
    public GameObject loadPanel;
    public GameObject initialPanel;
    void Start()
    {
        loadPanel.SetActive(false);
        saveLoadScript = GameObject.FindWithTag("Manager").GetComponent<SaveLoadScript>();
        bttn = gameObject.GetComponent<Button>();
        bttn.onClick.AddListener(SendInput);
    }

    // Update is called once per frame
    void SendInput()
    {
        loadPanel.SetActive(true);
        initialPanel.SetActive(false);
    }
}
