using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField]
    private Image Bar;
	
	void Update ()
    {
        Bar.fillAmount = ((float) Blackboard.PlaneControls.HP) / Blackboard.PlaneControls.maxHP;
	}
}
