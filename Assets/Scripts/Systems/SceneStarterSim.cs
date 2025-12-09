using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStarterSim : MonoBehaviour
{
    private void Start()
    {
        SceneFader.Instance.StartFadeIn();
    }
}
