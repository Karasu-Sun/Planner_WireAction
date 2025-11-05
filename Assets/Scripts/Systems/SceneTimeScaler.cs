using UnityEngine;

public class SceneTimeScaler : MonoBehaviour
{
    [Header("SceneTimeScaleActive")]
    public bool Scale = true;

    private void Update()
    {

        Time.timeScale = Scale ? 0 : 1;

    }
}
