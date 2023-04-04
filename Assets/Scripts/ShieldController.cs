using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class ShieldController : MonoBehaviour
{
    public GameObject rippleVfx;
    private Material mat;
    private static readonly int HitPoint = Shader.PropertyToID("_HitPoint");
    [SerializeField,Range(0,50)] //changed initial health to 50, phase 2 is 100.. for now....
    private float health = 50;
    public DroneController droneController;
    public Renderer shieldRenderer;
    private static readonly int ShieldHealth = Shader.PropertyToID("_ShieldHealth");
    private float timer;
    public MeshRenderer meshRenderer;
    public SphereCollider sphereCollider;
    [FormerlySerializedAs("shieldSpawnVFX")] public VisualEffect shieldVFX;

    public int phase;


    private void Start()
    {
        SpawnShield();
        phase = 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //new code added to add phases to the boss
        
        
        if (collision.gameObject.CompareTag("Bullet"))
        {
            health -= 10;
            if (health > 0) //if the shield is hit
            {
                shieldVFX.SendEvent("ShieldHit");
                shieldVFX.SetVector3("HitPoint", collision.contacts[0].point);
                if(phase == 1)
                {
                    shieldRenderer.material.SetFloat(ShieldHealth, health / 50);
                    shieldVFX.SetFloat("ShieldHealth", health / 50);
                }
                else if(phase == 2)
                {
                    shieldRenderer.material.SetFloat(ShieldHealth, health / 100);
                    shieldVFX.SetFloat("ShieldHealth", health / 100);
                }
                
            }
            else if(phase == 1) //If this is the first shield break
            {
                DisableShield();
                //play a sound effect
                //play an animation
                SpawnShield();
                shieldRenderer.material.SetFloat(ShieldHealth, 100 / 100);
                shieldVFX.SetFloat("ShieldHealth", 100 / 100);
                phase++;
            }
            else //if this is the second shield break
            {
                DisableShield();
                droneController.ShieldBreak();
                phase++;
            }
        }
    }

    private void OnApplicationQuit()
    {
        shieldRenderer.material.SetFloat(ShieldHealth, health/50);
    }

    private void SpawnShield()
    {
        health = 100;
        shieldRenderer.material.SetFloat(ShieldHealth, health/50);
        shieldVFX.SetFloat("ShieldHealth", health/100);
        shieldVFX.SendEvent("SpawnShield");
        sphereCollider.enabled = true;
        StartCoroutine(SpawnMainShield());
    }

    private void DisableShield()
    {
        sphereCollider.enabled = false;
        meshRenderer.enabled = false;
        shieldVFX.SendEvent("DespawnShield");

    }

    private IEnumerator SpawnMainShield()
    {
        yield return new WaitForSeconds(5);
        meshRenderer.enabled = true;
    }
}
