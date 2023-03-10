using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BrokenObjectController : MonoBehaviour
{
    private List<Transform> children = new();
    private float timer;

    void Start()
    {
        foreach (Transform child in transform)
        {
            children.Add(child);
            child.AddComponent<LifeTimeDespawn>();
        }
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        if (!(timer >= 1) || children.Count != 0) return;
        Destroy(gameObject);
        print($"Cleaned up {gameObject.name}");
    }
}
