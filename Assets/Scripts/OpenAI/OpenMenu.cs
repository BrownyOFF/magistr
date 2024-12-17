using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenMenu : MonoBehaviour
{
    private Button button;
    public GameObject panel;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenMenuFunc);
    }

    void OpenMenuFunc()
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
        }
    }
}
