using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DroneController : MonoBehaviour
{
    //Base Variables
    public Transform followTarget;
    [FormerlySerializedAs("ViewfinderTarget")] public Transform viewfinderTarget;
    [FormerlySerializedAs("invertViewfindAngle")] public bool invertViewfinding;
    [FormerlySerializedAs("lookSpeed")] public float viewfinderSpeed = 6;
    public float laserDistance;
    public bool fire = true;
    
    //attack variables
    public int phase = 1;
    public AttackVariables[] attackVariables = new AttackVariables[3];
    public float chargeAmount;
    private float savedChargeAmount;
    
    //health and shield info
    public int hitsToBreakShield = 3;
    [FormerlySerializedAs("timesBroken")] public int shieldBreaks;
    public bool shieldUp = true;
    public bool deathStarted;

    //bulletPrefab Variables
    public BulletManager.BulletStats droneBullet;
    [FormerlySerializedAs("BigBullet")] public BulletManager.BulletStats bigBullet;
    //Behavior Controls
    [FormerlySerializedAs("lookTarget")] public ViewfinderMode viewfinderMode = ViewfinderMode.Forward;
    [FormerlySerializedAs("stateMode")] public BehaviorStateMode behaviorStateMode = BehaviorStateMode.Idle;
    public MovementStateMode movementStateMode = MovementStateMode.Idle;

    //hidden variables
    private Vector3 prevPos;
    private Vector3 viewVector;
    private Vector3 moveDelta;
    private float burstTimer;
    private MovementStateMode savedMovementState;
    private BehaviorStateMode savedBehaviorState;
    private ViewfinderMode savedViewfinderMode;
    private Transform savedFollowTarget;
    [HideInInspector]public GameObject platformButton;

    //Bullet hit ten times show larger
    public bool smallBullet = true;
    private float bullet_tracker_count = 0;
    private float bullet_large_activateion = 10;

    //material info
    [FormerlySerializedAs("bodyShader")] public Material bodyMat;
    [FormerlySerializedAs("chargeShader")] public Material chargeMat;
    [FormerlySerializedAs("glowShader")] public Material glowMat;
    private static readonly int ChargeAmount = Shader.PropertyToID("_ChargeAmount");

    //audio variables
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
    }
    public enum MovementStateMode
    {
        FollowTarget,
        Patrol,
        Idle
    }
    
    //references
    public GameObject bulletPrefab;
    public Transform firePoint;
    
    public Transform positionController;
    public Transform rotationController;
    public VariableMovement wobbler;

    public ShieldController shield;
    public ShieldController invincibleShield;

    public Animator leftGen;
    public Animator rightGen;
    
    public LaserManager laser;
    public Animator animator;
    public List<GameObject> droneBodyParts;
    public List<Collider> enableAfterShieldBreak;
    private List<Rigidbody> droneBodyPartsRigidbodies = new();
    private List<Collider> droneBodyPartsColliders = new();
    private List<TransformMatcher> droneBodyPartsRotationMatchers = new();
    private static readonly int ReturnToIdle = Animator.StringToHash("Return to Idle");
    private static readonly int AnticipationSpeed = Animator.StringToHash("AnticipationSpeed");
    private static readonly int Fire = Animator.StringToHash("Fire");
    private static readonly int ReFire = Animator.StringToHash("ReFire");

    [Serializable]
    public class AttackVariables
    {
        public float burstRate = 1;//bursts per 10 seconds
        public float burstRateLarge = 1;
        public float burstAmount = 3;
        public float burstAmountLarge = 1;
        public float chargeTime = 3;
        public float chargeTimeLarge = 3;
        public float burstDuration = 2;
        public float burstDurationLarge = 1;
    }

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
        //manages the drone's behavior, destroys it if the body parts have de-spawned, and updates the laser.
        if(droneBodyParts.Count == 0) Destroy(gameObject);
        if(Math.Abs(laser.maxRange - laserDistance) > 0.05f) laser.maxRange = laserDistance;
        StateManager();
        //material updates to drone
        if(Math.Abs(savedChargeAmount - chargeAmount) > 0.01f)
        {
            bodyMat.SetFloat(ChargeAmount, chargeAmount);
            chargeMat.SetFloat(ChargeAmount, chargeAmount);
            glowMat.SetFloat(ChargeAmount, chargeAmount);
            savedChargeAmount = chargeAmount;
        }
    }

    public IEnumerator BurstFire()
    {
        float duration = smallBullet ? attackVariables[phase-1].burstDuration : attackVariables[phase-1].burstDurationLarge;
        float amount = smallBullet ? attackVariables[phase-1].burstAmount : attackVariables[phase-1].burstAmountLarge;
        for (int i = 0; i < amount; i++)
        {
            Shoot();
            if(i == (int)amount) break;
            yield return new WaitForSeconds(duration / amount);
            animator.SetTrigger(ReFire);
        }
        animator.SetTrigger(ReturnToIdle);
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
                if (fire)
                {
                    burstTimer += Time.deltaTime;
                    float rate = smallBullet? attackVariables[phase-1].burstRate : attackVariables[phase-1].burstRateLarge;
                    float chargeTime = smallBullet? attackVariables[phase-1].chargeTime : attackVariables[phase-1].chargeTimeLarge;
                    if (burstTimer > (10 / rate)-chargeTime)
                    {
                        animator.SetFloat(AnticipationSpeed, 1/chargeTime);
                        animator.SetTrigger(Fire);
                        burstTimer = 0;
                    }
                }

                break;
            case BehaviorStateMode.Dead:
                
                break;
            case BehaviorStateMode.Searching:
                
                break;
        }
        
        switch (movementStateMode)
        {
            case MovementStateMode.FollowTarget:
                if(followTarget != null) followTarget.rotation = Quaternion.identity;
                if (followTarget != savedFollowTarget)
                {
                    positionController.parent = followTarget;
                    positionController.localPosition = Vector3.zero;
                }
                break;
            case MovementStateMode.Patrol:
                break;
            case MovementStateMode.Idle:
                
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
                if (viewfinderTarget != null)
                {
                    viewVector = viewfinderTarget.position;
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
        GameObject spawnObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); // spawn the bullet at the fire point aimed forward
        //create a copy of bullet stats and assign it to the bullet stats of spawn object
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
                if(viewfinderTarget == null) viewfinderMode = ViewfinderMode.Free;
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
            case MovementStateMode.FollowTarget:
                if (followTarget == null)
                {
                    movementStateMode = MovementStateMode.Idle;
                    Debug.LogWarning("No follow target assigned, switching to idle state");
                }
                else
                {
                    positionController.parent = followTarget;
                    positionController.localPosition = Vector3.zero;
                }
                break;
            case MovementStateMode.Idle:
                print("movement state is idle, parent nullified");
                positionController.parent = null;
                break;
            case MovementStateMode.Patrol:
                
                break;
        }
    }
    private void InitializeBehaviorState(BehaviorStateMode stateToInitialize)
    {
        print($"state switched to {stateToInitialize}, initializing");
        burstTimer = 0;
        switch (stateToInitialize)
        {
            case BehaviorStateMode.Idle:
                movementStateMode = MovementStateMode.Idle;
                InitializeMovementState(movementStateMode);
                viewfinderMode = ViewfinderMode.Forward;
                InitializeLookState(viewfinderMode);
                break;
            case BehaviorStateMode.Chase:
                viewfinderMode = ViewfinderMode.Forward;
                break;
            case BehaviorStateMode.Attack:
                if(viewfinderTarget == null) behaviorStateMode = BehaviorStateMode.Idle;
                viewfinderMode = ViewfinderMode.GameObject;
                InitializeLookState(viewfinderMode);
                break;
            case BehaviorStateMode.Dead:

                break;
            case BehaviorStateMode.Searching:

                break;
            case BehaviorStateMode.LowerPlatform:
                viewfinderMode = ViewfinderMode.GameObject;
                movementStateMode = MovementStateMode.Idle;
                InitializeMovementState(movementStateMode);
                viewfinderTarget = platformButton.transform;
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
        var targetRotation = invertViewfinding ? Quaternion.LookRotation(pos - viewVector) : Quaternion.LookRotation(viewVector - pos);
        Debug.DrawRay(pos, viewVector-pos, Color.cyan);
        // Smoothly rotate towards the target point.
        //if(Vector3.Angle(targetRotation.eulerAngles,transform.rotation.eulerAngles) > 1)
        trans.rotation = Quaternion.Slerp(trans.rotation, targetRotation, viewfinderSpeed * Time.deltaTime);
    }

    private void Kill()
    {
        droneAudio.PlayOneShot(sfx[1]);
        behaviorStateMode = BehaviorStateMode.Dead;
        movementStateMode = MovementStateMode.Idle;
        viewfinderMode = ViewfinderMode.Free;
        foreach (var obj in droneBodyParts)
        {
            if(obj.name != "Body")
                obj.AddComponent<LifeTimeDespawn>();
        }
        foreach (var col in enableAfterShieldBreak)col.enabled = false;
        foreach (var collider1 in droneBodyPartsColliders)collider1.enabled = true;
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
        switch (phase)
        {
            case 1:
                StartCoroutine(PauseAttack(5));
                smallBullet = false;
                invincibleShield.gameObject.SetActive(true);
                leftGen.SetTrigger("Disable");
                break;
            case 2:
                StartCoroutine(PauseAttack(5));
                smallBullet = false;
                invincibleShield.gameObject.SetActive(true);
                invincibleShield.SpawnShield();
                rightGen.SetTrigger("Disable");
                break;
            case 3:
                
                break;
        }
        // shieldUp = false;
        // invincibleShield.gameObject.SetActive(true);
        // if(shieldBreaks == 3) foreach (var collider1 in enableAfterShieldBreak) collider1.enabled = true;
    }

    public IEnumerator PauseAttack(int seconds)
    {
        print("pausing attack for a few seconds");
        fire = false;
        yield return new WaitForSeconds(seconds);
        print("moving back to attack mode");
        fire = true;
    }

    private void Ten_hit_count()
    {
        bullet_tracker_count = bullet_tracker_count + 1;

        if(bullet_tracker_count >= bullet_large_activateion)
        {
            //change behavior state
            smallBullet = true;
        }
    }

    public void ExternalHit(Collision collision)
    {
        print("body received hit call from " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("DroneBullet") && !deathStarted && !shieldUp)
        {
            //calls the hit count script which will increment it until it reach the limit (10) which is when it shoot a big bullets
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
