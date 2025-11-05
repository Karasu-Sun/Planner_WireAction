using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace sei 
{
    public class SliderController : MonoBehaviour
    {
        [SerializeField] private Slider Slider;
        [SerializeField] private float leaveDistance = 0.3f;
        [SerializeField] private float policeSpeed = 0.1f;
        [SerializeField] private SceneManager sceneManager;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Slider.value -= Time.deltaTime * policeSpeed;
            if(Slider.value <= 0)
            {
                sceneManager.ChangeSceneResult();
            }
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Policeleave();
            }
        }

        private void Policeleave()
        {
            Slider.value -= leaveDistance;
        }
    }
}
