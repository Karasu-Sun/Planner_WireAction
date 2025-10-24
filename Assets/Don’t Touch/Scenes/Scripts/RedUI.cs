using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedUI : MonoBehaviour
{
    [SerializeField]
    Transform myTransform;

    [SerializeField]
    Transform Enemy01Transform;

    [SerializeField]
    Image UIRedUP;

    [SerializeField]
    Image UIRedDown;

    [SerializeField]
    Image UIRedRight;

    [SerializeField]
    Image UIRedLeft;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float z = Mathf.Abs(myTransform.position.z - Enemy01Transform.position.z);
        float x = Mathf.Abs(myTransform.position.x - Enemy01Transform.position.x);

        if(z < x) 
        { 
            if(myTransform.position.z < Enemy01Transform.position.z)
            {
                UIRedUP.color = new Color(UIRedUP.color.r, UIRedUP.color.g, UIRedUP.color.b, 0.5f);
                UIRedDown.color = new Color(UIRedDown.color.r, UIRedDown.color.g, UIRedDown.color.b, 0.0f);
                UIRedRight.color = new Color(UIRedRight.color.r, UIRedRight.color.g, UIRedRight.color.b, 0.0f);
                UIRedLeft.color = new Color(UIRedLeft.color.r, UIRedLeft.color.g, UIRedLeft.color.b, 0.0f);
            }
            else
            {
                UIRedUP.color = new Color(UIRedUP.color.r, UIRedUP.color.g, UIRedUP.color.b, 0.0f);
                UIRedDown.color = new Color(UIRedDown.color.r, UIRedDown.color.g, UIRedDown.color.b, 0.5f);
                UIRedRight.color = new Color(UIRedRight.color.r, UIRedRight.color.g, UIRedRight.color.b, 0.0f);
                UIRedLeft.color = new Color(UIRedLeft.color.r, UIRedLeft.color.g, UIRedLeft.color.b, 0.0f);
            }
        }
        else
        {
            if(myTransform.position.x < Enemy01Transform.position.x)
            {
                UIRedUP.color = new Color(UIRedUP.color.r, UIRedUP.color.g, UIRedUP.color.b, 0.0f);
                UIRedDown.color = new Color(UIRedDown.color.r, UIRedDown.color.g, UIRedDown.color.b, 0.0f);
                UIRedRight.color = new Color(UIRedRight.color.r, UIRedRight.color.g, UIRedRight.color.b, 0.5f);
                UIRedLeft.color = new Color(UIRedLeft.color.r, UIRedLeft.color.g, UIRedLeft.color.b, 0.0f);
            }
            else
            {
                UIRedUP.color = new Color(UIRedUP.color.r, UIRedUP.color.g, UIRedUP.color.b, 0.0f);
                UIRedDown.color = new Color(UIRedDown.color.r, UIRedDown.color.g, UIRedDown.color.b, 0.0f);
                UIRedRight.color = new Color(UIRedRight.color.r, UIRedRight.color.g, UIRedRight.color.b, 0.0f);
                UIRedLeft.color = new Color(UIRedLeft.color.r, UIRedLeft.color.g, UIRedLeft.color.b, 0.5f);
            }
        }
    }
}
