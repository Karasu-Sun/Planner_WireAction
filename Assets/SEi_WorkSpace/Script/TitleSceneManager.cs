using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private Sei.SceneManager sceneManager;
    void Update()
    {
        if(Keyboard.current.enterKey.wasPressedThisFrame)
        {
            sceneManager.ChangeSceneMain();
        }
    }
}
