using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedHUD : MonoBehaviour
{
    private Text speedText;

    private void Start()
    {
        speedText = GetComponentInChildren<Text>();
    }

    void Update ()
    {
        /*
        if (!speedText)
        {
            Debug.LogError("NO SPEED TEXT");
        }
        else if (!Blackboard.PlaneControls)
        {
            Debug.LogError("NO PLANE CONTROLS");
        }
        else
        {
            Debug.LogError("NO ");
        }*/
        speedText.text = "Speed: "+(int)Blackboard.PlaneControls.CurrentSpeed;
	}
}
