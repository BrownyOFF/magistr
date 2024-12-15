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
    public TMP_Dropdown dropdownText;
    public TMP_Dropdown dropdownImage;
    
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
            
            switch (PlayerPrefs.GetString("textModel"))
            {
                case "gpt-4o":
                    dropdownText.value = 0;
                    break;
                case "gpt-4o-mini":
                    dropdownText.value = 1;
                    break;
                case "gpt-3.5-turbo":
                    dropdownText.value = 2;
                    break;
            }
            switch (PlayerPrefs.GetString("imageModel"))
            {
                case "dall-e-3":
                    dropdownImage.value = 0;
                    break;
                case "dall-e-2":
                    dropdownImage.value = 1;
                    break;

            }
            
            panel.SetActive(true);
            isOpen = true;
        }
        else {
            panel.SetActive(false);
            isOpen = false;
        }
    }
}
