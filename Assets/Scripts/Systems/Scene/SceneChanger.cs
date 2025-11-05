using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Tooltip("シーン名一覧")]
    public List<string> sceneNames;

    public void StartChangeSceneByIndex(int index)
    {
        if (index >= 0 && index < sceneNames.Count)
        {
            ChangeScene(sceneNames[index]);
        }
        else
        {
            Debug.LogWarning("指定されたインデックスが範囲外: " + index);
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}