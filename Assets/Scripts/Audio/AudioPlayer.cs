using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    // OneShot
    private static bool copyRightPlayed = false;

    private void Update()
    {
        HandleSceneSounds();
    }

    private void HandleSceneSounds()
    {
        // CopyRight
        if (!copyRightPlayed && SceneStatus.Instance.GetStatus(SceneStatusType.IsCopyRight))
        {
            copyRightPlayed = true;
        }
    }
}