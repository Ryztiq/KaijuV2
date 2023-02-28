using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class DroneEnemyController : MonoBehaviour
{
    //variables
    private Vector3 prevPos;
    public float lookSpeed = 6;
    private Vector3 lookVec3;
    public Vector3 moveDelta;
    float timer;
    Vector3 originalPos;
    float waitTime = 1;
    public float followSpeed = 1;
    public bool invertLook;
    
    //references
    [HideInInspector]public GameObject objectUsingLookTarget;
    public GameObject ObjTarget;
    [HideInInspector]public Transform followTarget;

    private MovementStateMode savedMovementState;
    private BehaviorStateMode savedBehaviorState;
    private ViewfinderMode savedViewfinderMode;
    public enum ViewfinderMode
    {
        GameObject,
        Forward,
        None
    };
    [FormerlySerializedAs("lookTarget")] public ViewfinderMode viewfinderMode = ViewfinderMode.Forward;
    public enum BehaviorStateMode
    {
        Idle,
        Chase,
        Attack,
        Dead,
        Searching
    };
    [FormerlySerializedAs("stateMode")] public BehaviorStateMode behaviorStateMode = BehaviorStateMode.Idle;
    public enum MovementStateMode
    {
        followTarget,
        patrol,
        idle
    };
    public MovementStateMode movementStateMode = MovementStateMode.idle;

    // Start is called before the first frame update
    void Start()
    {
        if (objectUsingLookTarget == null)
            objectUsingLookTarget = gameObject;

        prevPos = transform.position;
        lookVec3 = objectUsingLookTarget.transform.position + objectUsingLookTarget.transform.forward.normalized;

        if(followTarget == null)followTarget = new GameObject($"{gameObject.name} Follow Target").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StateManager();
        // SetLookTarget();
        // LookForward();
    }

    private void StateManager()
    {
        switch (behaviorStateMode)
        {
            case BehaviorStateMode.Idle:
                break;
            case BehaviorStateMode.Chase:
                break;
            case BehaviorStateMode.Attack:
                break;
            case BehaviorStateMode.Dead:
                break;
            case BehaviorStateMode.Searching:
                break;
        }
        switch (movementStateMode)
        {
            case MovementStateMode.followTarget:
                break;
            case MovementStateMode.patrol:
                break;
            case MovementStateMode.idle:
                break;
        }

        //get a line showing how far the object moved each frame
        moveDelta = objectUsingLookTarget.transform.position - prevPos;
        switch (viewfinderMode)
        {
            case ViewfinderMode.Forward:
                //if the object moved more than 0.02 units, update a vector 3 showing which direction it's moving in.
                if (moveDelta.magnitude > 0.02f)
                    //center the origin of the movement-per-frame line on the game object and make a new vector3 from it.
                    lookVec3 = objectUsingLookTarget.transform.position + moveDelta.normalized;
                //update the previous known position of the object to the current location.
                prevPos = objectUsingLookTarget.transform.position;
                break;
            case ViewfinderMode.GameObject:
                if (ObjTarget != null)
                {
                    lookVec3 = ObjTarget.transform.position;
                }
                break;
        }
        LookToTarget();
        
        //state initialization
        
        // if(movementStateMode != savedMovementState)
        //     InitializeMovementState(movementStateMode);
        // if(behaviorStateMode != savedBehaviorState)
        //     InitializeBehaviorState(behaviorStateMode);
        if(viewfinderMode != savedViewfinderMode)
            InitializeLookState(viewfinderMode);
        savedViewfinderMode = viewfinderMode;
        savedBehaviorState = behaviorStateMode;
        savedMovementState = movementStateMode;
    }
    public void InitializeLookState(ViewfinderMode stateToInitialize)
    {
        print($"state switched to {stateToInitialize}, initializing");
        switch (stateToInitialize)
        {
            case ViewfinderMode.Forward:
                
                break;
            case ViewfinderMode.GameObject:
                
                break;
            case ViewfinderMode.None:
                
                break;
        }
    }

    public void InitializeMovementState(MovementStateMode stateToInitialize)
    {
        print($"state switched to {stateToInitialize}, initializing");
            switch (stateToInitialize)
        {
            // case MovementStateMode.followTarget:
            //     foreach (var obj in bodyObjects)
            //     {
            //         if(LeanTween.isTweening(obj)) LeanTween.cancel(obj);
            //         LeanTween.followDamp(obj.transform, followTarget, LeanProp.position, 0.5f/followSpeed);
            //     }
            //     break;
            // case MovementStateMode.idle:
            //     foreach (var obj in bodyObjects)
            //     {
            //         if(LeanTween.isTweening(obj)) LeanTween.cancel(obj);
            //     }                break;
            case MovementStateMode.patrol:
                
                break;
        }
    }

    private void LookToTarget()
    {
        var targetRotation = invertLook ? Quaternion.LookRotation(objectUsingLookTarget.transform.position - lookVec3) : Quaternion.LookRotation(lookVec3 - objectUsingLookTarget.transform.position);
        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(objectUsingLookTarget.transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        
        // //get a line in the direction that the object should be looking.
        // Vector3 lookAngle = (lookVec3 - objectUsingLookTarget.transform.position).normalized;
        // if(invertLook) lookAngle *= -1;
        // //get the difference between where the object should be looking and where it's actually looking.
        // Vector3 lookDif = lookAngle - objectUsingLookTarget.transform.forward.normalized;
        // //if that angle is greater than 1 degree, make the object look toward the preferred angle.
        // if (Vector3.Angle(objectUsingLookTarget.transform.forward.normalized, lookAngle) > 0.05f)
        // {
        //     objectUsingLookTarget.transform.forward += lookDif * (Time.deltaTime * lookSpeed);
        // }
    }
    public void SetLookObject(GameObject obj)
    {
        ObjTarget = obj;
        viewfinderMode = ViewfinderMode.GameObject;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(lookVec3,0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("TeleportBoundary"))
        {
            Destroy(gameObject);
        }
        // else if (other.gameObject.CompareTag("Destination"))
        // {
        //     SphereSpawner bossScript = GameObject.Find("BossBody").GetComponent<SphereSpawner>();
        //     LeanTween.cancel(gameObject);
        //     LeanTween.followDamp(transform, bossScript.sphereTargets[bossScript.spheres.IndexOf(gameObject)].transform,
        //         LeanProp.localPosition, 0.4f);
        // }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //TODO: Make the enemy shrink and dissappear when they hit the ground/major collision.
    }

    // public void DeathStart()
    // {
    //     print("Death Started");
    //     behaviorStateMode = BehaviorStateMode.Dead;
    //     lookTarget = LookTargets.Forward;
    //     GameObject boss = GameObject.Find("BossBody");
    //     // if (boss != null) GameObject.Find("BossBody").GetComponent<SphereSpawner>().spheres.Remove(gameObject);
    //     if(LeanTween.isTweening(gameObject)) LeanTween.cancel(gameObject);
    //     Rb.isKinematic = false;
    //     Rb.useGravity = true;
    //     deathInitialized = true;
    // }

    // private void OnGUI()
    // {
    //     //a button to run the death function
    //     if (GUI.Button(new Rect(10, 10, 100, 30), "Death"))
    //     {
    //         DeathStart();
    //     }
    // }
}
