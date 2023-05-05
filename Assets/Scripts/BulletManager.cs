using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class BulletManager : MonoBehaviour
{
    //variables
    public BulletStats bulletStats;
    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    private LifeTimeDespawn lifeTimeDespawn;
    public CapsuleCollider hitCollider;
    private VisualEffect bulletVFX;
    public AnimationCurve bulletLifeAlpha;


    [Serializable]public class BulletStats
    {
        public string tag = "Bullet";
        // public LayerMask layer;
        [FormerlySerializedAs("SphereSize")] public float sphereSize = 0.1f;
        [FormerlySerializedAs("Damage")] public int damage = 1;
        public float speed = 5f;
        [FormerlySerializedAs("LastingTime")] public float lastingTime = 2;
        public bool homing = false;
        public float homingAccuracy = 0.1f;
        public Transform target;
        public float inaccuracy = 1;
        public BulletStats(BulletStats bulletStats1)
        {
            tag = bulletStats1.tag;
            // layer = bulletStats1.layer;
            sphereSize = bulletStats1.sphereSize;
            damage = bulletStats1.damage;
            speed = bulletStats1.speed;
            lastingTime = bulletStats1.lastingTime;
            homing = bulletStats1.homing;
            homingAccuracy = bulletStats1.homingAccuracy;
            target = bulletStats1.target;
            inaccuracy = bulletStats1.inaccuracy;
        }
    }

    private void Start()
    {
        bulletVFX = GetComponent<VisualEffect>();
        //apply tag
        if(bulletStats.tag != null || bulletStats.tag != "") gameObject.tag = bulletStats.tag;
        //apply size
        transform.localScale = Vector3.one * bulletStats.sphereSize;
        //setup hit collider
        hitCollider.height = ((bulletStats.speed / 0.18f) / bulletStats.sphereSize)/100;
        hitCollider.center = new Vector3(0,0, Mathf.Clamp(hitCollider.height / 2, 0.01f, 99999));
        
        lifeTimeDespawn = GetComponent<LifeTimeDespawn>();
        lifeTimeDespawn.LastingTime = bulletStats.lastingTime;
        rb = GetComponent<Rigidbody>();
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
        
        //set VFX Alpha based on bullet lifetime
        float f = bulletLifeAlpha.Evaluate(lifeTimeDespawn.lifeTime / lifeTimeDespawn.LastingTime);
        bulletVFX.SetFloat("Alpha", f);
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
        print("Bullet collision:" + collision.gameObject.name);
        
        //trigger VFX
        bulletVFX.SendEvent("Collided");
        // bulletVFX.SetVector3("CollisionPosition", collision.contacts[0].point);
        bulletVFX.SetVector3("CollisionNormal", collision.contacts[0].normal);

        if (collision.gameObject.CompareTag("TennisRacket"))
        {
            rb.useGravity = true;
            lifeTimeDespawn.LastingTime = 50;
        }
        else
        {
            //set bullet lifetime to 1 second and reset its lifetime.
            lifeTimeDespawn.lifeTime = 0;
            lifeTimeDespawn.LastingTime = 1;
            
            if (collision.gameObject.CompareTag("Restart"))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            
            if(collision.gameObject.CompareTag("Invincible"))
                Destroy(gameObject);
        }
        if(bulletStats.homing)
        {
            bulletStats.homing = false;
        }
            
    }
}
