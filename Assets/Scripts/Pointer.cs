using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pointer : MonoBehaviour {

    public Transform direction;
    public float maxDistance = 20;
    private LineRenderer line;
    [SerializeField]
    private Transform Sparkles;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        /*RaycastHit hit;
        Ray raydirection = new Ray(direction.position, Vector3.Normalize((direction.position - transform.position)));
        if (Physics.Raycast(raydirection, out hit, maxDistance))
        {
            print("hit: "+hit.transform.gameObject.name);
            if (hit.collider.tag == "button")
            {
                print("This is a button: "+hit.transform.GetChild(0).GetComponent<Text>().text);
                Button btn = hit.transform.GetComponent<Button>();
                
            }
        }*/
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Sparkles.position = hit.point;
            Button hit_button = hit.transform.GetComponent<Button>();
            if (hit_button)
            {
                hit_button.onClick.Invoke();
            }
        }
        GetComponent<Joint>().enableCollision = true;
        Vector3[] positions = { transform.position, direction.position };
        line.SetPositions(positions);
    }
}
