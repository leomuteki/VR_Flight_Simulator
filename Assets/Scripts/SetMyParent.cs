using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMyParent : MonoBehaviour {

    public Transform Parent;

	void Start ()
    {
        transform.SetParent(Parent);
	}
	
}
