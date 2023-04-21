using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
        public string tag = "bulletPrefab";
        public float SphereSize = 0.1f;
        public int Damage = 1;
        public float speed = 5f;
        public float LastingTime = 2;
        public bool homing = false;
        public float homingAccuracy = 0.1f;
        public Transform target;
        public GameObject collisionVfx;
        public float inaccuracy = 1;
        public BulletStats(BulletStats bulletStats1)
        {
            tag = bulletStats1.tag;
            SphereSize = bulletStats1.SphereSize;
            Damage = bulletStats1.Damage;
            speed = bulletStats1.speed;
            LastingTime = bulletStats1.LastingTime;
            homing = bulletStats1.homing;
            homingAccuracy = bulletStats1.homingAccuracy;
            target = bulletStats1.target;
            collisionVfx = bulletStats1.collisionVfx;
            inaccuracy = bulletStats1.inaccuracy;
        }
    }

    private void Start()
    {
        //apply tag
        if(bulletStats.tag != null || bulletStats.tag != "") gameObject.tag = bulletStats.tag;
        //apply size
        transform.localScale = Vector3.one * bulletStats.SphereSize;
        //setup hit collider
        hitCollider.height = ((bulletStats.speed / 0.18f) / bulletStats.SphereSize)/100;
        hitCollider.center = new Vector3(0,0, Mathf.Clamp(hitCollider.height / 2, 0.01f, 99999));
        
        lifeTimeDespawn = GetComponent<LifeTimeDespawn>();
        lifeTimeDespawn.LastingTime = bulletStats.LastingTime;
        rb = GetComponent<Rigidbody>();
        transform.localScale = Vector3.one * bulletStats.SphereSize;
        Vector3 targetOffset = Vector3.one * Random.Range(-bulletStats.inaccuracy, bulletStats.inaccuracy);
        rb.velocity = (transform.forward + targetOffset) * bulletStats.speed;
    }
    private void FixedUpdate()
    {
        //make the object always face the direction it's moving if the velocity is greater than 0.1
        if (rb.velocity.magnitude > 0.9f && !bulletStats.homing)
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        if(bulletStats.target != null && bulletStats.homing)
        {
            //homing logic
            HomeToTarget();
        }
    }

    private void HomeToTarget()
    {
        float offset = Random.Range(1f, 1.15f);
        // Calculate the Perlin noise value based on time and speed
        float perlinValue = Mathf.PerlinNoise(Time.time * offset, 0f);

        // Multiply the Perlin noise value by the wobble scale to get the actual wobble amount
        float wobbleAmount = perlinValue * bulletStats.inaccuracy;

        // Create a wobble offset vector using Perlin noise in all three axes
        Vector3 wobbleOffset = new Vector3(
            Mathf.PerlinNoise(Time.time, 0f) * wobbleAmount,
            Mathf.PerlinNoise(0f, Time.time) * wobbleAmount,
            Mathf.PerlinNoise(Time.time, Time.time) * wobbleAmount);
        // Determine the direction towards the target
        Vector3 targetDirection = bulletStats.target.position - (transform.position+wobbleOffset);

        // Calculate the rotation towards the target using Quaternion.LookRotation
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Smoothly rotate towards the target using Quaternion.Slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * bulletStats.homingAccuracy);
        //use the magnitude of the rigidbody's velocity to accelerate it forward
        rb.velocity = transform.forward * rb.velocity.magnitude;
    }

    public void OnCollisionEnter(Collision collision)
    {
        // print("Collided with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Restart"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if(bulletStats.homing)
        {
            bulletStats.homing = false;
        }

        if (collision.gameObject.CompareTag("TennisRacket"))
        {
            rb.useGravity = true;
            lifeTimeDespawn.LastingTime = 50;
        }
    }
}
