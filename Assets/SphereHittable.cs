using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereHittable : Hittable
{
    public override void Hit()
    {
        Debug.Log("Sphere was hit!");
    }
}    

