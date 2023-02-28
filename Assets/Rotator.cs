using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 1f;
    private float angle;

    // Update is called once per frame
    void Update()
    {
        if (!transform.parent.GetComponent<Rigidbody>().useGravity)
        {
            angle += speed;
            transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z + angle));
        }
    }
}
