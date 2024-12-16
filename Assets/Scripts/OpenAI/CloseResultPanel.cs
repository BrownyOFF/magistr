using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseResultPanel : MonoBehaviour
{
    private Button bttn;
    public GameObject resultPanel;

    void Start()
    {
        bttn = gameObject.GetComponent<Button>();
        bttn.onClick.AddListener(Close);
    }

    void Close()
    {
        resultPanel.SetActive(false);
    }
}
