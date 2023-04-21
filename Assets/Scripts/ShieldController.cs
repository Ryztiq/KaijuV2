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
    [SerializeField,Range(0,100)]
    private float health = 100;
    public DroneController droneController;
    public Renderer shieldRenderer;
    private static readonly int ShieldHealth = Shader.PropertyToID("_ShieldHealth");
    private float timer;
    public MeshRenderer meshRenderer;
    public SphereCollider sphereCollider;
    [FormerlySerializedAs("shieldSpawnVFX")] public VisualEffect shieldVFX;

    private void Start()
    {
        SpawnShield();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bulletPrefab"))
        {
            health -= 10;
            if (health > 0)
            {
                shieldVFX.SendEvent("ShieldHit");
                shieldVFX.SetVector3("HitPoint", collision.contacts[0].point);
                shieldRenderer.material.SetFloat(ShieldHealth, health/100);
                shieldVFX.SetFloat("ShieldHealth", health/100);
            }
            else
            {
                DisableShield();
                droneController.ShieldBreak();
            }
        }
    }

    private void OnApplicationQuit()
    {
        shieldRenderer.material.SetFloat(ShieldHealth, health/100);
    }

    private void SpawnShield()
    {
        health = 100;
        shieldRenderer.material.SetFloat(ShieldHealth, health/100);
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
