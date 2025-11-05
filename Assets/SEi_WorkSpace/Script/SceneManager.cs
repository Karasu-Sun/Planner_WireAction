using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sei
{
    public class SceneManager : MonoBehaviour
    {
        public void ChangeSceneTitle()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Sei_work_TitleScene_Tentative");
        }
        public void ChangeSceneMain()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Sei_WorkScene");
        }

        public void ChangeSceneResult()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Sei_work_ResultScene_Tentative");
        }
    }
}