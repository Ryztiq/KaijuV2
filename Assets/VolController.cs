using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolController : MonoBehaviour
{
    public Rigidbody rb;
    public AudioSource audioSource;
    public float speedCeiling = 0.3f;
    public float speedFloor = 0;
    [SerializeField]private float speedPercentage;
    [HideInInspector]public Vector3 prevPosition = Vector3.zero;
    public float speed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        
        // Get the velocity of the tracked object
        speed = (rb.velocity.magnitude + rb.angularVelocity.magnitude)/2;;

        // Clamp the speed to the floor and ceiling values
        float clampedSpeed = Mathf.Clamp(speed, speedFloor, speedCeiling);

        // Calculate the percentage of where the speed is between the floor and ceiling values
        speedPercentage = Mathf.InverseLerp(speedFloor, speedCeiling, clampedSpeed);
        // audioSource.volume = speedPercentage;

        float val = (speedPercentage - audioSource.volume) * 0.5f;
        audioSource.volume += val;
        // if (speed != 0)
        // {
        //     float val = (speedPercentage - audioSource.volume) * 0.5f;
        //     audioSource.volume += val;
        // }
        // else audioSource.volume = 0;
    }

    public void OnGUI()
    {
        //a button to add random force and direction to the object 
        if (GUI.Button(new Rect(10, 10, 150, 100), "Add Force"))
        {
            // rb.AddForce(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * 1000);
            rb.AddForce(new Vector3(0,10,0), ForceMode.Impulse);
        }
        //a button to stop the object
        if (GUI.Button(new Rect(10, 120, 150, 100), "Stop"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        //a button to spin the object a random direction and amount
        if (GUI.Button(new Rect(10, 230, 150, 100), "Spin"))
        {
            rb.AddTorque(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * 1000);
        }
    }
}
