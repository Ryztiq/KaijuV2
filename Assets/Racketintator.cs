using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racketintator : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody Thisrb;
    public int bounceIntensity = 100;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("DroneBullet") || collision.gameObject.CompareTag("DroneBulletBig"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            Vector3 contactNormal = collision.contacts[0].normal;
            rb.AddForce(contactNormal*Thisrb.velocity.magnitude * bounceIntensity, ForceMode.Impulse);
        }
    }
}
