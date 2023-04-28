using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfHit : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.tag);
        if (collision.gameObject.CompareTag("DroneBullet"))
        {
            print("DID IT");
        }
    }

}
