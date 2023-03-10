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
    public List<Collider> enableAfterShieldBreak;
    public ShieldController shield;
    public bool deathStarted;
    public bool shieldUp = true;

    [HideInInspector] public GameObject followTarget;
    // Start is called before the first frame update
    void Start()
    {
        DroneSetup();
    }

    private void DroneSetup()
    {
        foreach (Transform child in transform)
        {
            //retrieve rigidbodies
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic) droneBodyPartsRigidbodies.Add(child.GetComponent<Rigidbody>());
            //retrieve child-matching components
            RotationMatcher matcher = child.GetComponent<RotationMatcher>();
            if (matcher != null) droneBodyPartsRotationMatchers.Add(child.GetComponent<RotationMatcher>());
        }

        foreach (var obj in droneBodyParts)
        {
            if (obj.name != "DroneArmature")
                RecursiveColliderFetch(obj.transform);
        }
    }

    public void RecursiveColliderFetch(Transform transform)
    {
        Collider col = transform.GetComponent<Collider>();
        if(col != null)droneBodyPartsColliders.Add(col);
        foreach (Transform child in transform)
        {
            RecursiveColliderFetch(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(droneBodyParts.Count == 0) Destroy(gameObject);

    }

    public void Kill()
    {
        foreach (var obj in droneBodyParts)
        {
            if(obj.name != "BodyMain")
                obj.AddComponent<LifeTimeDespawn>();
        }
        foreach (var col in enableAfterShieldBreak)col.enabled = false;
        foreach (var collider in droneBodyPartsColliders)collider.enabled = true;
        foreach (var rb in droneBodyPartsRigidbodies)rb.useGravity = true;
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

    public void SetFollowTarget()
    {
        
    }

    public void ShieldBreak()
    {
        shieldUp = false;
        foreach (var collider in enableAfterShieldBreak)
        {
            collider.enabled = true;
        }
    }

    public void ExternalHit(Collision collision)
    {
        print("body recieved hit call from " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Bullet") && !deathStarted && !shieldUp)
        {
            deathStarted = true;
            foreach (var col in enableAfterShieldBreak)
            {
                col.enabled = false;
            }
            Kill();
        }
    }
}
