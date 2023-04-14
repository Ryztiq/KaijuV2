using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLowerer : MonoBehaviour
{
    public List<GameObject> objectsToLower = new();

    public void LowerPlatform()
    {
        foreach (var obj in objectsToLower)
        {
            float pos = obj.transform.position.y-1.5f;
            LeanTween.moveY(obj, pos, 3);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("DroneBullet")) LowerPlatform();
    }
}
