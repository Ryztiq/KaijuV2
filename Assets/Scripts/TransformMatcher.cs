using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class TransformMatcher : MonoBehaviour
{
    private Rigidbody rigidbodyToRotate; // assign this in the Inspector
    public bool rotate;
    [FormerlySerializedAs("targetTransform")] public Transform rotateTarget; // assign this in the Inspector
    public bool follow;
    [FormerlySerializedAs("target")] public GameObject positionTarget;
    public float followSpeed;

    private void Awake()
    {
        rigidbodyToRotate = gameObject.GetComponent<Rigidbody>();
        rigidbodyToRotate.useGravity = false;

        if (follow)
        {
            if(positionTarget == null)
            {
                GameObject tempTarget = new GameObject($"{gameObject.name} Target");
                tempTarget.transform.parent = rotateTarget;
                tempTarget.transform.position = gameObject.transform.position;
                positionTarget = tempTarget;
            }
            LeanTween.followDamp(gameObject.transform, positionTarget.transform, LeanProp.position, 0.1f/followSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (rotate)
        {
            transform.localRotation = Quaternion.Slerp(rigidbodyToRotate.transform.localRotation, rotateTarget.localRotation, 12 * Time.deltaTime);
        }
    }

}
