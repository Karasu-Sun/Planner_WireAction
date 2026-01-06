using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStarter : MonoBehaviour
{

    [Header("ŽQÆ")]
    public SceneChanger sceneChanger_List;

    [Header("ˆÚ“®æ")]
    public int MainSceneIndexToLoad;
    public int EndSceneIndexToLoad;
    public int OverSceneIndexToLoad;

    private bool hasStarted = false;
    private bool hasEnded = false;
    private bool hasOvered = false;

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
                sceneChanger_List.StartChangeSceneByIndex(MainSceneIndexToLoad);
                SceneStatus.Instance.SetStatus(SceneStatusType.IsGameStart, false);

                hasStarted = false;
            });
        }

        if (SceneStatus.Instance.GetStatus(SceneStatusType.IsGameEnd))
        {
            if (hasEnded) return;
            hasEnded = true;

            SceneFader.Instance.StartFadeOut(() => {
                sceneChanger_List.StartChangeSceneByIndex(EndSceneIndexToLoad);
                SceneStatus.Instance.SetStatus(SceneStatusType.IsGameEnd, false);

                hasEnded = false;
            });
        }

        if (SceneStatus.Instance.GetStatus(SceneStatusType.IsGameOver))
        {
            if (hasOvered) return;
            hasOvered = true;

            SceneFader.Instance.StartFadeOut(() => {
                sceneChanger_List.StartChangeSceneByIndex(OverSceneIndexToLoad);
                SceneStatus.Instance.SetStatus(SceneStatusType.IsGameOver, false);

                hasOvered = false;
            });
        }
    }
}
