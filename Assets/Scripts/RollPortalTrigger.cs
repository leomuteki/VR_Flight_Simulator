using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollPortalTrigger : MonoBehaviour
{
    [SerializeField]
    private bool rollRight = true;
    [SerializeField]
    private float speed = 2;
    [SerializeField]
    private Transform PortalCamera;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "rollCollider")
        {
            if (rollRight)
            {
                PortalCamera.Rotate(Vector3.forward * speed);
            }
            else
            {
                PortalCamera.Rotate(-Vector3.forward * speed);
            }
        }
    }
}
