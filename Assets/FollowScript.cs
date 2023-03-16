using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//this means the script needs to have a rigidbody and collider to function, it will add one if it doesn't have one
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class FollowScript : MonoBehaviour
{
    //this script shows how to move objects toward one another in an exponential or constant speed, it also shows how to match the rotation of another object smoothly
    public Rigidbody rb;
    public Transform target;
    public bool moving;
    public bool constantSpeed;
    public float speed = 10;
    public bool homing;
    public bool rotating;
    public bool bounceOffOtherObjects;
    
    void FixedUpdate()
    {
        //if there is no target then the script will not run
        if (target == null) return;
        if (moving) MoveToTarget();
        if (rotating) RotateMatch();

    }

    private void MoveToTarget()
    {
        //calculates a vector from the object to the target
        Vector3 goal = target.position - transform.position;
        //sets the length of this vector to 1 to get a direction
        Vector3 direction = goal.normalized;
        //if the object is set to move a constant speed, it will move at the speed set in the inspector toward the target.
        if (homing && constantSpeed) rb.velocity = direction * speed;
        //otherwise, it will move toward the target at a speed that is proportional to the distance between the object and the target
        else rb.velocity = goal;
    }

    private void RotateMatch()
    {
        if (constantSpeed) //rotate to match the target's rotation at a constant speed.
        {
            // Calculate the rotation difference between this object and the target object
            Quaternion targetRotation = Quaternion.LookRotation(target.transform.forward, target.transform.up);
            Quaternion currentRotation = transform.rotation;
            //gets the difference in rotation, this isn't used here but can be useful for applications where you only want to rotate when the difference is above a threshold
            Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);

            // Apply the rotation at a constant speed
            float step = speed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, step);
        }
        //match the object's rotation smoothly if constant speed is false.
        else transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, 12 * Time.deltaTime);
    }
    
    public void OnCollisionEnter(Collision collision)
    {
        //makes the object bounce when it collides with an object besides the target
        if(collision.gameObject != target.gameObject && bounceOffOtherObjects)
        {
            //sets the homing value to false so the object doesn't keep moving toward the target, the if statement is so that this isn't called every frame
            if(homing) homing = false;
            //resets the velocity of the object to stop it.
            rb.velocity = Vector3.zero;
            //gets the direction of the collision and applies a force in the opposite direction
            rb.velocity = collision.GetContact(0).normal * speed;
        }
    }
    
    //debug functions
    public void OnDrawGizmos()
    {
        //draws a red line from the object to its target when gizmos are enabled
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target.position);
    }
}
