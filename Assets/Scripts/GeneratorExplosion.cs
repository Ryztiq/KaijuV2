using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Net;
using Unity.XR.CoreUtils;

public class GeneratorExplosion : MonoBehaviour
{
    //https://www.youtube.com/watch?v=AbHTpZmaVpk

    public bool hasExploded = false;
    public bool setColor = false;
    public ParticleSystem Explosion;
    public ParticleSystem Smoke;
    public AudioSource konosuba;
    
    
    public GameObject glowy;
    public Material emission;
    // Start is called before the first frame update
    void Start()
    {
        glowy = this.gameObject.transform.GetChild(0).gameObject;
        emission = glowy.GetComponent<MeshRenderer>().material;
        Explosion.Pause();
        Smoke.Pause();
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(setColor)
        //{
        //    emission.SetColor("_EmissionColor", Color.black * 0);
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasExploded)
        {
            if (collision.gameObject.CompareTag("DroneBullet"))
            {
                ColorChange();
                StartCoroutine(Explode());
                hasExploded = true;
            }
        }
    }
     
    private void ColorChange()
    {
        emission.DOColor(new Color(255, 52, 25), 2);
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(1.0f);


        Explosion.Play();
        Smoke.Play();
        konosuba.Play();

        yield return new WaitForSeconds(1.0f);
        //emission.SetColor("_EmissionColor", Color.black * 0);
        setColor = true;
        Destroy(emission);





    }
}
