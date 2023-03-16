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
    private bool initialized;
    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    public Transform target;

    [Serializable]public class BulletStats
    {
        public float SphereSize = 0.1f;
        public int Damage = 1;
        public float Speed = 5f;
        public float LastingTime = 0.5f;
        public float lifeTime = 2f;
        public bool homing = false;
        public float homingSpeed = 0.1f;
        public Transform target;
        public GameObject collisionVfx;
        public LayerMask bulletCollisions;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        transform.localScale = Vector3.one * bulletStats.SphereSize;
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.widthMultiplier = transform.localScale.x;
        StartCoroutine(DelayedFire());
    }

    public void Initialize(float sphereSize, int damage, float speed, float lastingTime)
    {
        bulletStats.SphereSize = sphereSize;
        bulletStats.Damage = damage;
        bulletStats.Speed = speed;
        bulletStats.LastingTime = lastingTime;
    }
    
    public void Initialize(float sphereSize, int damage, float speed)
    {
        bulletStats.SphereSize = sphereSize;
        bulletStats.Damage = damage;
        bulletStats.Speed = speed;
    }
    
    public void Initialize(float sphereSize, int damage, float speed, float lastingTime, Transform targetTrans, float homeSpeed)
    {
        bulletStats.SphereSize = sphereSize;
        bulletStats.Damage = damage;
        bulletStats.Speed = speed;
        bulletStats.LastingTime = lastingTime;
        bulletStats.target = targetTrans;
        bulletStats.homing = true;
        bulletStats.homingSpeed = homeSpeed;
    }

    public IEnumerator DelayedFire()
    {
        //wait for one frame
        yield return 0;
        //apply velocity
        if (!bulletStats.homing)
            rb.velocity = transform.forward * bulletStats.Speed;
        else
            HomeToTarget();
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

    public void HomeToTarget()
    {
        Vector3 goal = target.position - transform.position;
        Vector3 direction = goal.normalized;
        rb.velocity = direction * bulletStats.Speed;
    }

    private void LifeTimeDestroy()
    {
        bulletStats.lifeTime += Time.deltaTime;
        if (bulletStats.lifeTime > bulletStats.LastingTime && !LeanTween.isTweening(gameObject))
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
        if (bulletStats.homing == true)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
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
