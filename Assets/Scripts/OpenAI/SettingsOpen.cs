using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsOpen : MonoBehaviour
{
    public GameObject panel;
    public TMP_InputField APIKeyInput;
    public Slider tempSlid;
    public Slider maxSlid;

    private Button bttn;

    public bool isOpen = true;

    void Start()
    {
        bttn = this.gameObject.GetComponent<Button>();
        bttn.onClick.AddListener(updateAndShowHide);
        if(isOpen)
        {
            panel.SetActive(false);
            isOpen = false;
        }

    }

    void updateAndShowHide()
    {
        if(!isOpen)
        {
            APIKeyInput.text = PlayerPrefs.GetString("API");
            tempSlid.value =  PlayerPrefs.GetFloat("temp");
            maxSlid.value =  PlayerPrefs.GetInt("maxTok");
            panel.SetActive(true);
            isOpen = true;
        }
        else {
            panel.SetActive(false);
            isOpen = false;
        }
    }
}
