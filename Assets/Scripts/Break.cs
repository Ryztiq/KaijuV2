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
        print($"Shattering {gameObject.name}");
        GetComponent<Collider>().enabled = false;
        var velocity = rb.velocity;
        GameObject brokenObj = Instantiate(prefab, transform.position, transform.rotation);
        brokenObj.transform.localScale = transform.localScale;
        // foreach (Transform child in brokenObj.transform)
        // {
        //     child.GetComponent<Rigidbody>().AddForce(velocity* (velocityBreakCarryoverPercent/100), ForceMode.VelocityChange);
        // }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
        print($"{gameObject.name} Velocity: {Mathf.Clamp01(rb.velocity.magnitude / velocityToBreak)*100}% Collided with: {collision.gameObject.name}"); 
        if(otherRb != null) print($"{collision.gameObject.name} collider velocity: {Mathf.Clamp01(otherRb.velocity.magnitude/ velocityToBreak)*100}%");
        if (breakOnImpact && rb.velocity.magnitude > velocityToBreak) Shatter();
    }

    public void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRb = other.gameObject.GetComponent<Rigidbody>();
        print($"Object Velocity: {Mathf.Clamp01(rb.velocity.magnitude / velocityToBreak)*100}%"); 
        if(otherRb != null) print($"{other.gameObject.name} trigger velocity: {Mathf.Clamp01(otherRb.velocity.magnitude/ velocityToBreak)*100}%");
        if (breakOnImpact)
        {
            if(otherRb != null && otherRb.velocity.magnitude > velocityToBreak) 
                Shatter();
            else if(rb.velocity.magnitude > velocityToBreak)
                Shatter();
        }
    }
}
