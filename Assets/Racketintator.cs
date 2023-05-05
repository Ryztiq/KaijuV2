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
    public LayerMask hitMask;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("DroneBullet") || collision.gameObject.CompareTag("DroneBulletBig"))
        {
            //int LayerIgnoreRaycast = LayerMask.NameToLayer("Bullet");
            //collision.gameObject.layer = LayerIgnoreRaycast;
            audioSource.volume = 1;
            audioSource.PlayOneShot(hitSound);
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            Vector3 contactNormal = collision.contacts[0].normal;
            rb.AddForce(contactNormal*Thisrb.velocity.magnitude * bounceIntensity, ForceMode.Impulse);
        }
    }
}
