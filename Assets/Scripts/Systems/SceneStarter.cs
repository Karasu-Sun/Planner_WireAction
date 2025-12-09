using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStarter : MonoBehaviour
{

    [Header("ŽQÆ")]
    public SceneChanger sceneChanger_List;

    [Header("ˆÚ“®æ")]
    public int sceneIndexToLoad;

    private bool hasStarted = false;

    private void Start()
    {
        SceneFader.Instance.StartFadeIn();
    }

    private void Update()
    {
        if (SceneStatus.Instance.GetStatus(SceneStatusType.IsGameStart))
        {
            if (hasStarted) return;
            hasStarted = true;

            SceneFader.Instance.StartFadeOut(() => {
                sceneChanger_List.StartChangeSceneByIndex(sceneIndexToLoad);
                SceneStatus.Instance.SetStatus(SceneStatusType.IsGameStart, false);
                hasStarted = true;
            });
        }
    }
}
