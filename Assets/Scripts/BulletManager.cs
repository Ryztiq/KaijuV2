using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class BulletManager : MonoBehaviour
{
    //variables
    public BulletStats bulletStats;
    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    private LifeTimeDespawn lifeTimeDespawn;
    public CapsuleCollider hitCollider;
    private bool initialized = false;


    [Serializable]public class BulletStats
    {
        public float SphereSize = 0.1f;
        public int Damage = 1;
        public float speed = 5f;
        public float LastingTime = 2;
        public bool homing = false;
        public float homingAccuracy = 0.1f;
        public Transform target;
        public GameObject collisionVfx;
        public LayerMask bulletCollisions;

        public BulletStats(BulletStats bulletStats1)
        {
            SphereSize = bulletStats1.SphereSize;
            Damage = bulletStats1.Damage;
            speed = bulletStats1.speed;
            LastingTime = bulletStats1.LastingTime;
            homing = bulletStats1.homing;
            homingAccuracy = bulletStats1.homingAccuracy;
            target = bulletStats1.target;
            collisionVfx = bulletStats1.collisionVfx;
            bulletCollisions = bulletStats1.bulletCollisions;
        }
    }

    private void Awake()
    {
        lifeTimeDespawn = GetComponent<LifeTimeDespawn>();
        lifeTimeDespawn.LastingTime = bulletStats.LastingTime;
        rb = GetComponent<Rigidbody>();
        transform.localScale = Vector3.one * bulletStats.SphereSize;
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.widthMultiplier = transform.localScale.x;
        StartCoroutine(DelayedFire());
    }

    public IEnumerator DelayedFire()
    {
        //wait for one frame
        yield return 0;
        //apply size
        transform.localScale = Vector3.one * bulletStats.SphereSize;
        //setup hit collider
        hitCollider.height = ((bulletStats.speed / 0.18f) / bulletStats.SphereSize)/100;
        hitCollider.center = new Vector3(0,0, hitCollider.height / 2);
        //apply velocity
        rb.velocity = transform.forward * bulletStats.speed;
        initialized = true;
    }
    private void FixedUpdate()
    {
        if (initialized)
        {
            //make the object always face the direction it's moving if the velocity is greater than 0.1
            if (rb.velocity.magnitude > 0.9f && !bulletStats.homing)
                transform.rotation = Quaternion.LookRotation(rb.velocity);
            else if(bulletStats.target != null)
            {
                //homing logic
                HomeToTarget();
            }
            trailRenderer.widthMultiplier = transform.localScale.x;
            RaycastHit hit;
        }
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

    public void HomeToTarget()
    {
        // Determine the direction towards the target
        Vector3 targetDirection = bulletStats.target.position - transform.position;

        // Calculate the rotation towards the target using Quaternion.LookRotation
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Smoothly rotate towards the target using Quaternion.Slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * bulletStats.homingAccuracy);
        //use the magnitude of the rigidbody's velocity to accelerate it forward
        rb.velocity = transform.forward * rb.velocity.magnitude;
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
        if(bulletStats.homing)
        {
            bulletStats.homing = false;
        }
        if (bulletStats.collisionVfx!= null)Instantiate(bulletStats.collisionVfx, transform.position, transform.rotation);
        // Destroy(gameObject);
    }
}
