using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResultSceneChanger : MonoBehaviour
{
    [SerializeField] private SceneManager sceneManager;
    private void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            sceneManager.ChangeSceneTitle();
        }
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            sceneManager.ChangeSceneMain();
        }
    }
}