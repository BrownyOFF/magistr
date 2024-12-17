using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Networking;

public class SettingsApply : MonoBehaviour
{
    public string APIKey;
    public int temp;
    public int maxTok;

    public GameObject resultPanel;
    public TMP_InputField APIKeyInput;
    public Slider tempSlid;
    public Slider maxSlid;
    public TMP_Dropdown dropdownText;
    public TMP_Dropdown dropdownImage;
    
    public SendRequest sendRequest;

    private Button bttn;
    public Button bttnApiCheck;
    void Start()
    {
        bttnApiCheck.onClick.AddListener(Wrapper);
        resultPanel.SetActive(false);
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

    public void Wrapper()
    {
        StartCoroutine(CheckAPIKey());
    }
    public IEnumerator CheckAPIKey()
    {
        var jsonData = JsonConvert.SerializeObject(new { model = "gpt-3.5-turbo", messages = new List<object> { new { role = "system", content = "Test" } }, max_tokens = 1 });
        using (UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {APIKeyInput.text}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                sendRequest.ShowPanelForLog("API Key is valid", true);
                Debug.Log("API Key is valid.");
            }
            else if (request.responseCode == 401)
            {
                sendRequest.ShowPanelForLog("API Key is invalid", true);
                Debug.LogError("Invalid API Key.");
            }
        }
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
                PlayerPrefs.SetString("textModel", "gpt-4-turbo");
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
