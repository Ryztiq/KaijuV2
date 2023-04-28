using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GeneratorExplosion : MonoBehaviour
{
    bool hasExploded = false;
    public ParticleSystem Explosion;
    public ParticleSystem Smoke;
    public AudioSource konosuba;

    public Material emission;
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

    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.gameObject.CompareTag("DroneBullet"))
        {
            ColorChange();
            StartCoroutine(Explode());
        }
    }

    private void ColorChange()
    {
        emission.DOColor(new Color(255, 52, 25), 2);
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(2);
        if (!hasExploded)
        {


            Explosion.Play();
            Smoke.Play();
            konosuba.Play();
            emission.color = Color.black;


        }
    }
}
