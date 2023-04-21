using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittable : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
        if(otherRb != null && collision.gameObject.CompareTag("bulletPrefab"))
        {
            Hit();
        }
        if(collision.gameObject.CompareTag("Ground"))
            print("Object hit ground");
    }

    public virtual void Hit()
    {
        Debug.Log("Hit");
    }
}
