using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class OpenExplorer : MonoBehaviour
{
    private Button bttn;
    void Start()
    {
        bttn = gameObject.GetComponent<Button>();
        bttn.onClick.AddListener(OpenFolder);
    }
    void OpenFolder()
    {
        string folderPath = Application.persistentDataPath;
        if (System.IO.Directory.Exists(folderPath))
        {
            #if UNITY_STANDALONE_WIN
            Process.Start("explorer.exe", folderPath.Replace("/", "\\"));
            #elif UNITY_STANDALONE_LINUX
            Process.Start("xdg-open", folderPath);
            #endif
        }
        else
        {
            Debug.LogError("Папка не існує: " + folderPath);
        }
    }
}
