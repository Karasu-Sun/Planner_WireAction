using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static SoundManager;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField]
    private int index = 0;

    [SerializeField]
    private bool playOneShot = false;

    [SerializeField]
    private Vector3 position = new Vector3(0f, 30f, 0f);

    private void Update()
    {
        if (SceneStatus.Instance.GetStatus(SceneStatusType.IsItemCatch))
        {
            if (playOneShot)
                return;

            playOneShot = true;
            SoundManager.Instance.PlaySEAtPosition(0, position, 10);
        }

        if (SceneStatus.Instance.GetStatus(SceneStatusType.IsGameStart))
        {
            SoundManager.Instance.StopAllLoopSE();
            playOneShot = false;
        }
    }
}
