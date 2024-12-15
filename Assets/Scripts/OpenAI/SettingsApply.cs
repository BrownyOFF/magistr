using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class SettingsApply : MonoBehaviour
{
    public string APIKey;
    public int temp;
    public int maxTok;

    public TMP_InputField APIKeyInput;
    public Slider tempSlid;
    public Slider maxSlid;
    public TMP_Dropdown dropdownText;
    public TMP_Dropdown dropdownImage;
    
    public SendRequest sendRequest;

    private Button bttn;
    void Start()
    {
        sendRequest = GameObject.FindWithTag("Manager").GetComponent<SendRequest>();
        bttn = gameObject.GetComponent<Button>();
        bttn.onClick.AddListener(ApplySett);
        if (!PlayerPrefs.HasKey("temp"))
        {
            ApplyDefault();
        }
    }

    void ApplyDefault()
    {
        PlayerPrefs.SetFloat("temp", 1);
        PlayerPrefs.SetInt("maxTok", 100);
        PlayerPrefs.SetString("textModel", "gpt-4o-mini");
        PlayerPrefs.SetString("imageModel", "dall-e-2");
        PlayerPrefs.SetInt("familyMode", 0);
    }
    
    void ApplySett()
    {
        PlayerPrefs.SetString("API", APIKeyInput.text.ToString());
        PlayerPrefs.SetFloat("temp", (float)tempSlid.value);
        PlayerPrefs.SetInt("maxTok", (int)maxSlid.value);
        
        switch (dropdownText.value)
        {
            case 0:
                PlayerPrefs.SetString("textModel", "gpt-4o");
                break;
            case 1:
                PlayerPrefs.SetString("textModel", "gpt-4o-mini");
                break;
            case 2:
                PlayerPrefs.SetString("textModel", "gpt-3.5-turbo");
                break;
        }
        switch (dropdownImage.value)
        {
            case 0:
                PlayerPrefs.SetString("imageModel", "dall-e-3");
                break;
            case 1:
                PlayerPrefs.SetString("imageModel", "dall-e-2");
                break;
        }
        
        sendRequest.apiKey = PlayerPrefs.GetString("API");
        Debug.Log(PlayerPrefs.GetString("API") + "\n" + PlayerPrefs.GetFloat("temp") + "\n" + PlayerPrefs.GetInt("maxTok"));
    }
}
