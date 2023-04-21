using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnelController : MonoBehaviour
{
    public LaserGunManager laserGunManager;
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("bulletPrefab"))
        {
            Destroy(other.gameObject);
            laserGunManager.SendHaptics();
        }
    }
}
