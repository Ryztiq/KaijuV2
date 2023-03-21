using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BrokenObjectController : MonoBehaviour
{
    private float timer;
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(.8f, 1.2f);
        audioSource.Play();
    }

    void Start()
    {
        foreach (Transform child in transform) child.AddComponent<LifeTimeDespawn>();
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1 && transform.childCount == 0)
        {
            Destroy(gameObject);
            print($"Cleaned up {gameObject.name}");
        }
    }
}
