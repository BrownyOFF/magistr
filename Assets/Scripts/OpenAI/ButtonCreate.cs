using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCreate : MonoBehaviour
{
    Button bttn;
    public GameObject panelOpen;
    public GameObject panelClose;
    void Start()
    {
        bttn = gameObject.GetComponent<Button>();
        panelOpen.SetActive(false);
        bttn.onClick.AddListener(Script);

    }

    void Script()
    {
        panelOpen.SetActive(true);
        panelClose.SetActive(false);
    }
}    
