using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorExplosion : MonoBehaviour
{
    bool hasExploded = false;
    public ParticleSystem Explosion;
    public ParticleSystem Smoke;
    // Start is called before the first frame update
    void Start()
    {
        Explosion.Stop();
        Smoke.Stop();
        Smoke.gameObject.SetActive(false);
        Explosion.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explode()
    {
        if(!hasExploded)
        {

            Smoke.gameObject.SetActive(true);
            Explosion.gameObject.SetActive(true);
            Explosion.Play();
            Smoke.Play();


        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(gameObject.CompareTag("DroneBulletBig"))
        {
            Explode();
        }
    }
}
