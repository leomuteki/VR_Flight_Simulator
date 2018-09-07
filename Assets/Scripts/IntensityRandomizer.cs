using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensityRandomizer : MonoBehaviour
{
    private Light myLight;
    private float timeGap = 0.1f;
    private float lastTime = 0;
    private Vector2 IntensityRange = new Vector2(10, 50);
    private float IntensityGap;

	void Start ()
    {
        myLight = GetComponent<Light>();
        IntensityGap = IntensityRange.y - IntensityRange.x;
	}

    private void OnEnable()
    {
        lastTime = Time.time;
    }

    void Update ()
    {
        if ((Time.time - lastTime) >= timeGap)
        {
            myLight.intensity = Random.Range(IntensityRange.x + IntensityGap, IntensityRange.y);
            lastTime = Time.time;
        }
        else if ((Time.time - lastTime) >= (timeGap / 2))
        {
            myLight.intensity = Random.Range(IntensityRange.x, IntensityRange.y - IntensityGap);
        }


    }
}
