using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public bool isFollowing = true;
    public Transform Parent;
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if (isFollowing)
        {
            transform.position = Parent.position;
            transform.rotation = Parent.rotation;
        }
	}
}
