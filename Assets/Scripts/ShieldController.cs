using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public GameObject rippleVfx;

    private Material mat;
    private static readonly int HitPoint = Shader.PropertyToID("_HitPoint");
    public float health = 100;
    public DroneController droneController;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            var ripple = Instantiate(rippleVfx, transform) as GameObject;
            var psr = ripple.GetComponent<ParticleSystemRenderer>();
            mat = psr.material;
            mat.SetVector(HitPoint, collision.contacts[0].point);
            print("destroying ripple in 2s");
            Destroy(ripple, 2f);
            health -= 20;
            if(health <= 0)
            {
                Destroy(gameObject);
                droneController.ShieldBreak();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
