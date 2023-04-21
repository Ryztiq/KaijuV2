using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racketintator : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("DroneBullet") || collision.gameObject.CompareTag("BigDroneBullet"))
        {
            collision.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
