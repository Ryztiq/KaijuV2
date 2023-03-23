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
    public Renderer shieldRenderer;
    private static readonly int ShieldHealth = Shader.PropertyToID("_ShieldHealth");
    
    private void Start()
    {
        shieldRenderer.material.SetFloat(ShieldHealth, health/100);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            var ripple = Instantiate(rippleVfx, transform) as GameObject;
            var psr = ripple.GetComponent<ParticleSystemRenderer>();
            mat = psr.material;
            shieldRenderer.material.SetVector(HitPoint, collision.contacts[0].point);
            print("destroying ripple in 2s");
            Destroy(ripple, 2f);
            health -= 10f;
            shieldRenderer.material.SetFloat(ShieldHealth, health/100);
            if(health <= 0)
            {
                Destroy(gameObject);
                droneController.ShieldBreak();
            }
        }
    }

    private void OnApplicationQuit()
    {
        shieldRenderer.material.SetFloat(ShieldHealth, health/100);
    }
}
