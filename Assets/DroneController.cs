using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public List<GameObject> droneBodyParts;
    public List<Rigidbody> droneBodyPartsRigidbodies;
    public List<Collider> droneBodyPartsColliders;
    public List<RotationMatcher> droneBodyPartsRotationMatchers;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var obj in droneBodyParts)
        {
            droneBodyPartsColliders.Add(obj.transform.GetComponentInChildren<Collider>());
            foreach (Transform child in obj.transform)
            {
                Collider col = child.gameObject.GetComponent<Collider>();
                if(col != null)droneBodyPartsColliders.Add(col);
            }
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if(rb != null)droneBodyPartsRigidbodies.Add(obj.GetComponent<Rigidbody>());
            RotationMatcher matcher = obj.GetComponent<RotationMatcher>();
            if(matcher != null)droneBodyPartsRotationMatchers.Add(obj.GetComponent<RotationMatcher>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(droneBodyParts[0] == null) Destroy(gameObject);
    }

    public void Kill()
    {
        foreach (var obj in droneBodyParts)
        {
            LeanTween.cancel(obj);
            obj.AddComponent<LifeTimeDespawn>();
        }

        foreach (var collider in droneBodyPartsColliders)
        {
            collider.enabled = true;
        }

        foreach (var rb in droneBodyPartsRigidbodies)
        {
            rb.useGravity = true;
        }

        foreach (var matcher in droneBodyPartsRotationMatchers)
        {
            matcher.rotate = false;
            matcher.follow = false;
        }
    }

    public void OnGUI()
    {
        //a button to kill the drone
        if (GUI.Button(new Rect(10, 10, 100, 30), "Kill Drone"))
        {
            Kill();
        }
    }
}
