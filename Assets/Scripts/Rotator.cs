using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 axis = new Vector3(0, 0, 1);
    public bool local = true;

    public bool sine = false;
    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = (transform.parent == null)? null : transform.parent.GetComponent<Rigidbody>();
        if (rb!= null && rb.useGravity) return;
        if (sine)
        {
            transform.Rotate(new Vector3(axis.x, axis.y, axis.z)*Mathf.Sin(Time.time), local ? Space.Self : Space.World);
        }
        else
            transform.Rotate(new Vector3(axis.x, axis.y, axis.z), local ? Space.Self : Space.World);
    }
}
