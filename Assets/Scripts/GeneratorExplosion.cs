using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorExplosion : MonoBehaviour
{
    bool hasExploded = false;
    public ParticleSystem Explosion;
    public ParticleSystem Smoke;
    public AudioSource konosuba;
    // Start is called before the first frame update
    void Start()
    {
        
        Explosion.Pause();
        Smoke.Pause();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explode()
    {
        if(!hasExploded)
        {

            
            Explosion.Play();
            Smoke.Play();
            konosuba.Play();


        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("DroneBullet"))
        {
            Explode();
        }
    }
}
