using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorExplosion : MonoBehaviour
{
    public Animator animator;
    private static readonly int Explode1 = Animator.StringToHash("Explode");
    public void Explode()
    {
        animator.SetTrigger(Explode1);
        LifeTimeDespawn despan = transform.parent.gameObject.AddComponent<LifeTimeDespawn>();
        despan.waitForRB = false;
        despan.LastingTime = 15;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("DroneBulletBig"))
        {
            Explode();
        }
    }
}
