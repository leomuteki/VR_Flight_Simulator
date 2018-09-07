using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalControl : MonoBehaviour {

    [SerializeField]
    private float speed = 2f;

    private void Awake()
    {
        Blackboard.PortalCam = transform;
        Blackboard.PortalCamReset = transform.rotation;
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(-Vector3.right * speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(Vector3.right * speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(-Vector3.up * speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up * speed);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Rotate(-Vector3.forward * speed);
        }
        if (Input.GetKey(KeyCode.C))
        {
            transform.Rotate(Vector3.forward * speed);
        }
    }
}
