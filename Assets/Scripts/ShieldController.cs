using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class ShieldController : MonoBehaviour
{
    private Material mat;
    private static readonly int HitPoint = Shader.PropertyToID("_HitPoint");
    [SerializeField, Range(0, 100)]
    public float health = 100;
    public DroneController droneController;
    public Renderer shieldRenderer;
    private static readonly int ShieldHealth = Shader.PropertyToID("_ShieldHealth");
    private float timer;
    public MeshRenderer meshRenderer;
    public SphereCollider sphereCollider;
    [FormerlySerializedAs("shieldSpawnVFX")] public VisualEffect shieldVFX;
    public bool invincible;
    private void OnEnable()
    {
        SpawnShield();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DroneBullet"))
        {
            if(!invincible)
                if (droneController != null)
                {
                    health -= (int)(100 / droneController.hitsToBreakShield) + 1;
                    
                    if (health > 0)
                    {
                        shieldVFX.SendEvent("ShieldHit");
                        shieldVFX.SetVector3("HitPoint", collision.contacts[0].point);

                        float healthPercent = health / 100;
                        print($"Set shield material property to {healthPercent}");
                        shieldRenderer.material.SetFloat(ShieldHealth, healthPercent);
                        shieldVFX.SetFloat("ShieldHealth", healthPercent);
                    }
                    else
                    {
                        DisableShield();
                        droneController.ShieldBreak();
                    }

                }

        }
    }

    private void OnApplicationQuit()
    {
        shieldRenderer.material.SetFloat(ShieldHealth, (health / 100));
    }

    public void SpawnShield()
    {
        health = 100;
        droneController.droneAudio.PlayOneShot(droneController.sfx[2]);
        shieldRenderer.material.SetFloat(ShieldHealth, health/100);
        shieldVFX.SetFloat("ShieldHealth", health/100);
        shieldVFX.SendEvent("SpawnShield");
        sphereCollider.enabled = true;
        StartCoroutine(SpawnMainShield());
    }

    public void DisableShield()
    {
        droneController.droneAudio.PlayOneShot(droneController.sfx[3]);
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
