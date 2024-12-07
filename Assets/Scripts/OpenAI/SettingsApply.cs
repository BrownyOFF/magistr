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

    private Button bttn;
    void Start()
    {
        bttn = gameObject.GetComponent<Button>();
        bttn.onClick.AddListener(ApplySett);
    }

    void ApplySett()
    {
        PlayerPrefs.SetString("API", APIKeyInput.text.ToString());
        PlayerPrefs.SetFloat("temp", (float)tempSlid.value);
        PlayerPrefs.SetInt("maxTok", (int)maxSlid.value);
        Debug.Log(PlayerPrefs.GetString("API") + "\n" + PlayerPrefs.GetFloat("temp") + "\n" + PlayerPrefs.GetInt("maxTok"));
    }
}
