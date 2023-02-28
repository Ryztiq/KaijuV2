using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSender : MonoBehaviour
{
    public DroneController droneController;
    
    private void OnCollisionEnter(Collision collision)
    {
        print("Hit by " + collision.gameObject.name);
        droneController.ExternalHit(collision);
    }

    public void OnTriggerEnter(Collider other)
    {
        print("Hit by " + other.gameObject.name);
    }
}
