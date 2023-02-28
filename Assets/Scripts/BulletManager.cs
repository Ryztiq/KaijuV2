using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class BulletManager : MonoBehaviour
{
    //variables
    public float SphereSize;
    public int Damage;
    public float Speed;
    public float LastingTime;
    private float lifeTime;
    public LayerMask bulletCollisions;
    //References
    public Rigidbody rb;
    [FormerlySerializedAs("hitEffect")] public GameObject collisionVfx;
    private TrailRenderer trailRenderer;

    private void Awake()
    {
        transform.localScale = Vector3.one * SphereSize;
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.widthMultiplier = transform.localScale.x;
        rb.velocity = transform.forward * Speed*10;
    }

    private void FixedUpdate()
    {
        trailRenderer.widthMultiplier = transform.localScale.x;
        LifeTimeDestroy();
        RaycastHit hit;

        // Physics.SphereCast(transform.position, SphereSize, Vector3.forward, out hit);
        // try
        // {
        //     print("HIT");
        // }
        // catch
        // {
        //     print("Something went wrong");
        // }
        //TODO: Refactor the above code into the checksphere below as it's more performant.

        //if (Physics.CheckSphere(transform.position, 0.1f, bulletCollisions))
        //{

        //    Instantiate(hitEffect, transform.position, transform.rotation);
        //    Destroy(this.gameObject);
        //}

        //I like cheese, cheese is good, all hail cheese
    }

    private void LifeTimeDestroy()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime > LastingTime && !LeanTween.isTweening(gameObject))
        {
            LeanTween.scale(gameObject, Vector3.zero, 1).setEaseInExpo();
        }
        if (transform.localScale.x < 0.01f)
            Destroy(gameObject);
    }

    //[ExecuteAlways]
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, transform.localScale.magnitude*1.1f);
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            print("hit enemy");
            // other.GetComponent<BallEnemyController>().DeathStart();
        }
        if (collision.gameObject.CompareTag("Restart"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if(collision.gameObject.CompareTag("Shield"))
            Destroy(gameObject);
        if(collisionVfx!= null)Instantiate(collisionVfx, transform.position, transform.rotation);
        // Destroy(gameObject);
    }
}
