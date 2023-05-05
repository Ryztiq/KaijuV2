using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTimeDespawn : MonoBehaviour
{
    public float LastingTime = 2;
    public float despawnTime = 0.5f;
    [HideInInspector]public float lifeTime;
    private Rigidbody rb;
    public bool waitForRB = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        LeanTween.cancel(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // if (rb != null)
        // {
        //     rb.velocity *= 0.9f;
        //     rb.angularVelocity *= 0.99f;
        // }
        LifeTimeDestroy();
    }
    
    private void LifeTimeDestroy()
    {
        lifeTime += Time.deltaTime;
        if (rb != null && lifeTime >= 1 && waitForRB)
        {
            if (rb.velocity.magnitude < 0.1f && rb.angularVelocity.magnitude < 0.1f && rb.useGravity && !rb.isKinematic)
            {
                rb.isKinematic = true;
                lifeTime = 0;
            }
            if (rb.isKinematic && lifeTime > LastingTime && !LeanTween.isTweening(gameObject)) LeanTween.scale(gameObject, Vector3.zero, despawnTime).setEaseInExpo();
        }
        else if (lifeTime > LastingTime && !LeanTween.isTweening(gameObject)) LeanTween.scale(gameObject, Vector3.zero, despawnTime).setEaseInExpo();
        if (transform.localScale.x < 0.01f) Destroy(gameObject);
    }
}
