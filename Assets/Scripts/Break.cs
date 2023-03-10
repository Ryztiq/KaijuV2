using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Break : MonoBehaviour
{
    public bool breakOnImpact;
    public float velocityToBreak = 10;
    public GameObject prefab;
    public float velocityBreakCarryoverPercent = 50;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Shatter()
    {
        GetComponent<Collider>().enabled = false;
        var velocity = rb.velocity;
        GameObject brokenObj = Instantiate(prefab, transform.position, transform.rotation);
        brokenObj.transform.localScale = transform.localScale;
        foreach (Transform child in brokenObj.transform)
        {
            child.GetComponent<Rigidbody>().AddForce(velocity* (velocityBreakCarryoverPercent/100), ForceMode.VelocityChange);
        }
        AddLifeTimeScript(brokenObj.transform);
        Destroy(this.GameObject());
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
        print($"Object Velocity: {Mathf.Clamp01(rb.velocity.magnitude / velocityToBreak)*100}%"); 
        if(otherRb != null) print($"External Object:{collision.gameObject.name} Velocity: {Mathf.Clamp01(otherRb.velocity.magnitude/ velocityToBreak)*100}%");
        if (breakOnImpact)
        {
            if(otherRb != null && rb.velocity.magnitude > velocityToBreak) 
                Shatter();
            else if(rb.velocity.magnitude > velocityToBreak)
                Shatter();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRb = other.gameObject.GetComponent<Rigidbody>();
        print($"Object Velocity: {Mathf.Clamp01(rb.velocity.magnitude / velocityToBreak)*100}%"); 
        if(otherRb != null) print($"External Object:{other.gameObject.name} Velocity: {Mathf.Clamp01(otherRb.velocity.magnitude/ velocityToBreak)*100}%");
        if (breakOnImpact)
        {
            if(otherRb != null && rb.velocity.magnitude > velocityToBreak) 
                Shatter();
            else if(rb.velocity.magnitude > velocityToBreak)
                Shatter();
        }
    }

    public void AddLifeTimeScript(Transform trans)
    {
        foreach (Transform child in trans)
        {
            child.AddComponent<LifeTimeDespawn>();
            AddLifeTimeScript(child);
        }
    }
}
