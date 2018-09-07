using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
    private Text myText;

    private void Start()
    {
        myText = transform.GetComponentInChildren<Text>();
    }

    void Update ()
    {
        myText.text = "Score: " + Blackboard.PlaneControls.Score;
	}
}
