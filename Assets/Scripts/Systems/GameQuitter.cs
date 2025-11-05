using UnityEditor;
using UnityEngine;

public class GameQuitter : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
                QuitGame();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}