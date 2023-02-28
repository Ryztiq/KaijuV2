using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTimeDespawn : MonoBehaviour
{
    public float LastingTime = 4;
    private float lifeTime;

    // Update is called once per frame
    void Update()
    {
        LifeTimeDestroy();
    }
    
    private void LifeTimeDestroy()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime > LastingTime && !LeanTween.isTweening(gameObject))
        {
            LeanTween.scale(gameObject, Vector3.zero, 1).setEaseInExpo();
        }
        if (transform.localScale.x < 0.01f)
            Destroy(gameObject);
    }
}
