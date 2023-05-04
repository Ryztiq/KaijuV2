using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DroneController : MonoBehaviour
{
    //Base Variables
    public float lookSpeed = 6;
    public float laserDistance;
    [FormerlySerializedAs("fireRate")] public float burstRate = 1;//bursts per 10 seconds
    public float burstAmount = 5;
    public float chargeTime = 1;
    public float burstDuration = 1;
    [HideInInspector]public float chargeAmount;
    public bool invertViewfindAngle;
    public Transform ViewfinderTarget;
    public Transform followTarget;
    //bulletPrefab Variables
    public BulletManager.BulletStats droneBullet;
    public BulletManager.BulletStats BigBullet;
    //Behavior Controls
    [FormerlySerializedAs("lookTarget")] public ViewfinderMode viewfinderMode = ViewfinderMode.Forward;
    [FormerlySerializedAs("stateMode")] public BehaviorStateMode behaviorStateMode = BehaviorStateMode.Idle;
    public MovementStateMode movementStateMode = MovementStateMode.idle;

    [HideInInspector]public bool shieldUp = true;
    [HideInInspector]public bool deathStarted;
    private Vector3 prevPos;
    private Vector3 viewVector;
    private Vector3 moveDelta;
    [SerializeField]private float timer;
    [SerializeField]private MovementStateMode savedMovementState;
    [SerializeField]private BehaviorStateMode savedBehaviorState;
    [SerializeField]private ViewfinderMode savedViewfinderMode;
    private Transform savedFollowTarget;
    public GameObject platformButton;
    public int timesBroken;

    //Bullet hit ten times show larger
    private bool big_bullet_shoot_toggle = false;
    private float bullet_tracker_count = 0;
    private float bullet_large_activateion = 10;

    //Barrett's Additions
    [FormerlySerializedAs("bodyShader")] public Material bodyMat;
    [FormerlySerializedAs("chargeShader")] public Material chargeMat;
    [FormerlySerializedAs("glowShader")] public Material glowMat;

    public AudioSource droneAudio;
    public List<AudioClip> sfx;
    //attack
    //die
    //shield

    public enum ViewfinderMode
    {
        GameObject,
        Forward,
        Free
    };
    public enum BehaviorStateMode
    {
        Idle,
        Chase,
        Attack,
        Dead,
        Searching,
        LowerPlatform
    };
    public enum MovementStateMode
    {
        followTarget,
        patrol,
        idle
    };
    //references
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform positionController;
    public Transform rotationController;
    public ShieldController shield;
    public ShieldController invincibleShield;
    public LaserManager laser;
    public Animator animator;
    public List<GameObject> droneBodyParts;
    public List<Collider> enableAfterShieldBreak;
    private List<Rigidbody> droneBodyPartsRigidbodies = new();
    private List<Collider> droneBodyPartsColliders = new();
    private List<TransformMatcher> droneBodyPartsRotationMatchers = new();
    private static readonly int ChargeAmount = Shader.PropertyToID("_ChargeAmount");
    public VariableMovement wobbler;

    // Start is called before the first frame update
    void Start()
    {
        DroneSetup();
        prevPos = transform.position;
        viewVector = positionController.position + positionController.forward.normalized;
        
        DroneCleanup();

        //Error Catches
        if (rotationController == null) rotationController = positionController;
    }

    private void DroneCleanup()
    {
        //material Reset
        bodyMat.SetFloat(ChargeAmount, 0);
        chargeMat.SetFloat(ChargeAmount, 0);
        glowMat.SetFloat(ChargeAmount, 0);
    }

    private void DroneSetup()
    {
        foreach (Transform child in transform)
        {
            //get all child rigidbodies of children
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic) droneBodyPartsRigidbodies.Add(child.GetComponent<Rigidbody>());
            //retrieve transform-matching components in children
            TransformMatcher matcher = child.GetComponent<TransformMatcher>();
            if (matcher != null) droneBodyPartsRotationMatchers.Add(child.GetComponent<TransformMatcher>());
        }
        
        //retrieve all colliders in children
        foreach (var obj in droneBodyParts)
        {
            if (obj.name != "DroneArmature")
                RecursiveColliderFetch(obj.transform);
        }
    }

    private void RecursiveColliderFetch(Transform trans)
    {
        Collider col = trans.GetComponent<Collider>();
        if(col != null)droneBodyPartsColliders.Add(col);
        foreach (Transform child in trans) RecursiveColliderFetch(child);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //manages the drone's behavior, destroys it if the body parts have despawned, and updates the laser.
        if(droneBodyParts.Count == 0) Destroy(gameObject);
        if(Math.Abs(laser.maxRange - laserDistance) > 0.05f) laser.maxRange = laserDistance;
        StateManager();
        //Barrett's Change
        bodyMat.SetFloat(ChargeAmount, chargeAmount);
        chargeMat.SetFloat(ChargeAmount, chargeAmount);
        glowMat.SetFloat(ChargeAmount, chargeAmount);
    }

    public IEnumerator BurstFire()
    {
        for (int i = 0; i < burstAmount; i++)
        {
            Shoot();
            yield return new WaitForSeconds(burstDuration/ burstAmount);
        }
        animator.SetTrigger("Return to Idle");
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
                timer += Time.deltaTime;
                //drone fires
                if(big_bullet_shoot_toggle == true)
                {
                    //shoot Big bullet
                    // Shoot();
                    big_bullet_shoot_toggle = false;
                }
                else if (timer > (10 / burstRate)-chargeTime)
                {
                    animator.SetFloat("AnticipationSpeed", 1/chargeTime);
                    animator.SetTrigger("Fire");
                    timer = 0;
                }
                break;
            case BehaviorStateMode.Dead:
                
                break;
            case BehaviorStateMode.Searching:
                
                break;
        }
        
        switch (movementStateMode)
        {
            case MovementStateMode.followTarget:
                if(followTarget != null) followTarget.rotation = Quaternion.identity;
                if (followTarget != savedFollowTarget)
                {
                    positionController.parent = followTarget;
                    positionController.localPosition = Vector3.zero;
                }
                break;
            case MovementStateMode.patrol:
                break;
            case MovementStateMode.idle:
                
                break;
        }
        //get a line showing how far the object moved each frame
        moveDelta = positionController.position - prevPos;
        
        switch (viewfinderMode)
        {
            case ViewfinderMode.Forward:
                //if the object moved more than 0.02 units, update a vector 3 showing which direction it's moving in.
                if (moveDelta.magnitude > 0.02f)
                    //center the origin of the movement-per-frame line on the game object and make a new vector3 from it.
                    viewVector = positionController.position + moveDelta.normalized;
                //update the previous known position of the object to the current location.
                prevPos = positionController.position;
                
                rotationController.localRotation = positionController.localRotation;
                LookToTarget(positionController);
                
                break;
            case ViewfinderMode.GameObject:
                if (ViewfinderTarget != null)
                {
                    viewVector = ViewfinderTarget.position;
                    LookToTarget(rotationController);
                }
                else
                    viewfinderMode = ViewfinderMode.Free;
                break;
        }

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

    private void Shoot()
    {
        droneAudio.pitch = Random.Range(0.9f, 1.1f);
        droneAudio.PlayOneShot(sfx[0]);
        droneAudio.pitch = 1;
        GameObject spawnObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); // spawn the object at the mouse click position with the correct rotation
        //create a copy of bulletstats and assign it to the bulletstats of spawnobject
        spawnObject.GetComponent<BulletManager>().bulletStats = new BulletManager.BulletStats(droneBullet);
    }

    private void InitializeLookState(ViewfinderMode stateToInitialize)
    {
        print($"state switched to {stateToInitialize}, initializing");
        switch (stateToInitialize)
        {
            case ViewfinderMode.Forward:
                rotationController.localRotation = Quaternion.identity;
                break;
            case ViewfinderMode.GameObject:
                if(ViewfinderTarget == null) viewfinderMode = ViewfinderMode.Free;
                positionController.rotation = Quaternion.identity;
                    break;
            case ViewfinderMode.Free:
                
                break;
        }
    }
    private void InitializeMovementState(MovementStateMode stateToInitialize)
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
                    positionController.parent = followTarget;
                    positionController.localPosition = Vector3.zero;
                }
                break;
            case MovementStateMode.idle:
                print("movement state is idle, parent nullified");
                positionController.parent = null;
                break;
            case MovementStateMode.patrol:
                
                break;
        }
    }
    private void InitializeBehaviorState(BehaviorStateMode stateToInitialize)
    {
        print($"state switched to {stateToInitialize}, initializing");
        timer = 0;
        switch (stateToInitialize)
        {
            case BehaviorStateMode.Idle:
                viewfinderMode = ViewfinderMode.Forward;
                InitializeLookState(viewfinderMode);
                movementStateMode = MovementStateMode.idle;
                InitializeMovementState(movementStateMode);
                break;
            case BehaviorStateMode.Chase:
                viewfinderMode = ViewfinderMode.Forward;
                break;
            case BehaviorStateMode.Attack:
                if(ViewfinderTarget == null) behaviorStateMode = BehaviorStateMode.Idle;
                viewfinderMode = ViewfinderMode.GameObject;
                break;
            case BehaviorStateMode.Dead:

                break;
            case BehaviorStateMode.Searching:

                break;
            case BehaviorStateMode.LowerPlatform:
                viewfinderMode = ViewfinderMode.GameObject;
                movementStateMode = MovementStateMode.idle;
                InitializeMovementState(movementStateMode);
                ViewfinderTarget = platformButton.transform;
                droneBullet.target = platformButton.transform;
                droneBullet.homing = true;
                Shoot();
                break;
        }
    }

    private void LookToTarget(Transform trans)
    {
        //gets an angle between the object's forward vector and the vector between the object and the target.
        Vector3 pos = trans.position;
        var targetRotation = invertViewfindAngle ? Quaternion.LookRotation(pos - viewVector) : Quaternion.LookRotation(viewVector - pos);
        Debug.DrawRay(pos, viewVector-pos, Color.cyan);
        // Smoothly rotate towards the target point.
        //if(Vector3.Angle(targetRotation.eulerAngles,transform.rotation.eulerAngles) > 1)
        trans.rotation = Quaternion.Slerp(trans.rotation, targetRotation, lookSpeed * Time.deltaTime);
    }

    public void Kill()
    {
        droneAudio.PlayOneShot(sfx[1]);
        behaviorStateMode = BehaviorStateMode.Dead;
        movementStateMode = MovementStateMode.idle;
        viewfinderMode = ViewfinderMode.Free;
        foreach (var obj in droneBodyParts)
        {
            if(obj.name != "Body")
                obj.AddComponent<LifeTimeDespawn>();
        }
        foreach (var col in enableAfterShieldBreak)col.enabled = false;
        foreach (var collider in droneBodyPartsColliders)collider.enabled = true;
        foreach (var rb in droneBodyPartsRigidbodies)rb.useGravity = true;
        foreach (var matcher in droneBodyPartsRotationMatchers)
        {
            matcher.rotate = false;
            matcher.follow = false;
        }
    }

    public void OnGUI()
    {
        //a button to kill the drone
        if (GUI.Button(new Rect(10, 10, 100, 30), "Kill Drone"))
        {
            Kill();
        }
        //a button to kill the drone
        if (GUI.Button(new Rect(110, 10, 100, 30), "Lower Platform"))
        {
            behaviorStateMode = BehaviorStateMode.LowerPlatform;
        }
    }

    public void ShieldBreak()
    {
        droneAudio.PlayOneShot(sfx[2]);
        shieldUp = false;
        invincibleShield.gameObject.SetActive(true);
        if(timesBroken == 3)
        foreach (var collider in enableAfterShieldBreak) collider.enabled = true;
    }

    private void Ten_hit_count()
    {
        bullet_tracker_count = bullet_tracker_count + 1;

        if(bullet_tracker_count >= bullet_large_activateion)
        {
            //change behaviro state
            big_bullet_shoot_toggle = true;
        }
    }

    public void ExternalHit(Collision collision)
    {
        print("body recieved hit call from " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("DroneBullet") && !deathStarted && !shieldUp)
        {
            //calls the hit count script wich will incriment it until it reach the limit (10) whcih is when it shoot a big bullets
            Ten_hit_count();
            deathStarted = true;
            foreach (var col in enableAfterShieldBreak)col.enabled = false;
            Kill();
        }
    }
    
    //debug

    public void OnApplicationQuit()
    {
        DroneCleanup();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(viewVector,0.5f);
    }
}
