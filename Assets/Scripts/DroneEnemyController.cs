using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
public class DroneEnemyController : MonoBehaviour
{
    //variables
    private Vector3 prevPos;
    public float lookSpeed = 6;
    private Vector3 lookVec3;
    public Vector3 moveDelta;
    float timer;
    float shootTimer = 0;
    public float shootTimerMax = 5;
    bool canShoot = true;


    Vector3 originalPos;
    public bool invertLook;
    
    //references
    [FormerlySerializedAs("objectUsingLookTarget")] [HideInInspector]public Transform transformController;
    public Transform ObjTarget;
    public Transform followTarget;
    public GameObject Lazer;
    public GameObject ShootAngleReference;

    private MovementStateMode savedMovementState;
    private BehaviorStateMode savedBehaviorState;
    private ViewfinderMode savedViewfinderMode;
    private Transform savedFollowTarget;
    public enum ViewfinderMode
    {
        GameObject,
        Forward,
        Free
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
        if (transformController == null)transformController = gameObject.transform;
        prevPos = transform.position;
        lookVec3 = transformController.position + transformController.forward.normalized;
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
                InitializeAttackState();
                break;
            case BehaviorStateMode.Dead:
                break;
            case BehaviorStateMode.Searching:
                break;
        }
        switch (movementStateMode)
        {
            case MovementStateMode.followTarget:
                if(followTarget != null) followTarget.rotation = quaternion.Euler(Vector3.zero);
                if (followTarget != savedFollowTarget)
                {
                    transformController.parent = followTarget;
                    transformController.localPosition = Vector3.zero;
                }
                break;
            case MovementStateMode.patrol:
                break;
            case MovementStateMode.idle:
                break;
        }

        //get a line showing how far the object moved each frame
        moveDelta = transformController.position - prevPos;
        switch (viewfinderMode)
        {
            case ViewfinderMode.Forward:
                {
                    //if the object moved more than 0.02 units, update a vector 3 showing which direction it's moving in.
                    if (moveDelta.magnitude > 0.02f)
                        //center the origin of the movement-per-frame line on the game object and make a new vector3 from it.
                        lookVec3 = transformController.position + moveDelta.normalized;
                    //update the previous known position of the object to the current location.
                    prevPos = transformController.position;
                    break;
                }
            case ViewfinderMode.GameObject:
                {
                    if (ObjTarget != null)
                    {
                        lookVec3 = ObjTarget.position;
                    }
                    else
                    {
                        viewfinderMode = ViewfinderMode.Free;
                    }
                    break;
                }
        }
        LookToTarget();
        
        //state initialization
        
        if(movementStateMode != savedMovementState)
            InitializeMovementState(movementStateMode);
         if(behaviorStateMode != savedBehaviorState)
             InitializeBehaviorState(behaviorStateMode);
        if(viewfinderMode != savedViewfinderMode)
            InitializeLookState(viewfinderMode);
        
        //update saved states
        savedViewfinderMode = viewfinderMode;
        savedBehaviorState = behaviorStateMode;
        savedMovementState = movementStateMode;
        savedFollowTarget = followTarget;
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
            case ViewfinderMode.Free:
                
                break;
        }
    }

    public void InitializeMovementState(MovementStateMode stateToInitialize)
    {
        print($"state switched to {stateToInitialize}, initializing");
            switch (stateToInitialize)
        {
            case MovementStateMode.followTarget:
                if (followTarget == null)
                {
                    movementStateMode = MovementStateMode.idle;
                    Debug.LogWarning("No follow target assigned, switching to idle state");
                }
                else
                {
                    transformController.parent = followTarget;
                    transformController.localPosition = Vector3.zero;
                }
                break;
            case MovementStateMode.idle:
                transformController.parent = null;
                break;
            case MovementStateMode.patrol:
                
                break;
        }
    }
    
    public void InitializeBehaviorState(BehaviorStateMode stateToInitialize)
    {
        print($"state switched to {stateToInitialize}, initializing");
        switch (stateToInitialize)
        {
            case BehaviorStateMode.Idle:
                Debug.Log("Is Idle");
                break;
            case BehaviorStateMode.Chase:
                Debug.Log("Is Chasing");

                break;
            case BehaviorStateMode.Attack:
                Debug.Log("Is Attacking");

                break;
            case BehaviorStateMode.Dead:
                Debug.Log("Is Dead");

                break;
            case BehaviorStateMode.Searching:
                Debug.Log("Is Searching");

                break;
        }
    }

    public void InitializeAttackState()
    {
        if (canShoot)
        {
            int choice = UnityEngine.Random.Range(0, 2);
            Debug.Log(choice);
            if (ObjTarget != null)
            {
                Lazer.GetComponent<BulletManager>().target = ObjTarget.transform;
                Lazer.GetComponent<BulletManager>().bulletStats.homing = true;
                Debug.Log("'Homing' Shot");
                Instantiate(Lazer, ShootAngleReference.transform.position, ShootAngleReference.transform.rotation);
            }
            else
            {
                Lazer.GetComponent<BulletManager>().target = null;
                Lazer.GetComponent<BulletManager>().bulletStats.homing = false;

                Debug.Log("Shooting Forward");
                Instantiate(Lazer, ShootAngleReference.transform.position, ShootAngleReference.transform.rotation);

            }
            canShoot = false; 
        }
        else
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootTimerMax)
            {
                canShoot = true;
                shootTimer = 0;
            }
        }

    }

    private void LookToTarget()
    {
        var targetRotation = invertLook ? Quaternion.LookRotation(transformController.position - lookVec3) : Quaternion.LookRotation(lookVec3 - transformController.position);
        Debug.DrawRay(transformController.position, lookVec3-transformController.position, Color.cyan);
        // Smoothly rotate towards the target point.
        //if(Vector3.Angle(targetRotation.eulerAngles,transform.rotation.eulerAngles) > 1)
        transform.rotation = Quaternion.Slerp(transformController.rotation, targetRotation, lookSpeed * Time.deltaTime);
        
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
        ObjTarget = obj.transform;
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

    public void OnGUI()
    {
        //a button to set the follow target to the player.
        if (GUI.Button(new Rect(110, 10, 100, 30), "Follow Player"))
        {
            followTarget = GameObject.Find("FollowCube").transform;
            movementStateMode = MovementStateMode.followTarget;
        }
    }
}
