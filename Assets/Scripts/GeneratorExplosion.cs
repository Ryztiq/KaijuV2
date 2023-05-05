using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorExplosion : MonoBehaviour
{
    bool hasExploded = false;
    public ParticleSystem Explosion;
    public ParticleSystem Smoke;
    public AudioSource konosuba;

    public void Explode()
    {
        if (hasExploded) return;
        Explosion.Play();
        Smoke.Play();
        konosuba.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("DroneBullet"))
        {
            Explode();
        }
    }
}
