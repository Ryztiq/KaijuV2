using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racketintator : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody Thisrb;
    public int bounceIntensity = 100;
    public AudioClip hitSound;
    public AudioSource audioSource;
    [SerializeField]private Vector3 savedPos;

    public void FixedUpdate()
    {
        if(savedPos!= transform.position) savedPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("DroneBullet") || collision.gameObject.CompareTag("DroneBulletBig"))
        {
            audioSource.PlayOneShot(hitSound);
            
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.useGravity = true;

            float speed = (transform.position - savedPos).magnitude;
            Vector3 contactNormal = collision.contacts[0].normal;
            rb.AddForce(contactNormal*speed * bounceIntensity, ForceMode.Impulse);
        }
    }
}
