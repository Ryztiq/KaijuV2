using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RotationMatcher : MonoBehaviour
{
    private Rigidbody rigidbodyToRotate; // assign this in the Inspector
    public Transform targetTransform; // assign this in the Inspector
    public bool rotate;
    public bool follow;
    public GameObject target;
    public float followSpeed;

    private void Awake()
    {
        rigidbodyToRotate = gameObject.GetComponent<Rigidbody>();
        rigidbodyToRotate.useGravity = false;

        if (follow)
        {
            if(target == null)
            {
                GameObject tempTarget = new GameObject($"{gameObject.name} Target");
                tempTarget.transform.parent = targetTransform;
                tempTarget.transform.position = gameObject.transform.position;
                target = tempTarget;
            }
            LeanTween.followDamp(gameObject.transform, target.transform, LeanProp.position, 0.1f/followSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (rotate)
        {
            // // Get the rotation difference between the rigidbody and the target transform
            // Quaternion deltaRotation = targetTransform.rotation * Quaternion.Inverse(rigidbodyToRotate.rotation);
            //
            // // Convert the delta rotation into an axis-angle representation
            // Vector3 axis;
            // float angle;
            // deltaRotation.ToAngleAxis(out angle, out axis);
            // if(angle > 5)
            // {
            //     // Apply torque to the rigidbody using its current angular velocity
            //     Vector3 angularVelocity = rigidbodyToRotate.angularVelocity;
            //     Vector3 torque = axis * angle * Mathf.Deg2Rad / Time.fixedDeltaTime;
            //     rigidbodyToRotate.AddTorque(torque - rigidbodyToRotate.inertiaTensorRotation * Vector3.Cross(angularVelocity, Vector3.Scale(Vector3.Scale(rigidbodyToRotate.inertiaTensor, rigidbodyToRotate.inertiaTensorRotation.eulerAngles * -1), angularVelocity)), ForceMode.Impulse);
            // }
            // else if(angle != 0)
            // {
            //     rigidbodyToRotate.angularVelocity /= 2;
            // }
            // if(rigidbodyToRotate.angularVelocity.magnitude < 0.01f)
            // {
            //     rigidbodyToRotate.angularVelocity = Vector3.zero;
            // }
            transform.localRotation = Quaternion.Slerp(rigidbodyToRotate.transform.localRotation, targetTransform.localRotation, 12 * Time.deltaTime);
        }
    }

}
