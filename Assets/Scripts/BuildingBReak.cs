using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBReak : MonoBehaviour
{
    private Rigidbody rb;
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (rb.isKinematic)
        {
            if(collision.gameObject.CompareTag("bulletPrefab")) rb.isKinematic = false;
            if(collision.gameObject.CompareTag("Debris") && !collision.gameObject.GetComponent<Rigidbody>().isKinematic) rb.isKinematic = false;
        }
    }
}
