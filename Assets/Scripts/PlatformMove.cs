using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    public bool isAutomated;
    public bool isCurrentlyAutoMoving;
    public float destination;
    public float totalDistanceTraveled;
    [SerializeField] float incrementAdded;
    public float dropSpeed = 5;
    [SerializeField] int lives; 
    public GameObject platform;
    Vector3 targetPosition;



    // Start is called before the first frame update
    void Start()
    {
        isAutomated = true;
        isCurrentlyAutoMoving = false;
        lives = 3;
        dropSpeed = 5;
        destination = -30f;
        incrementAdded = (destination - platform.transform.position.y) / 3;
        targetPosition = new Vector3(platform.transform.position.x, platform.transform.position.y, platform.transform.position.z);
       
    }

    // Update is called once per frame
    void Update()
    {
                   
    }

    private void FixedUpdate()
    {
        if(isCurrentlyAutoMoving)
        {
            Debug.Log("Is Automated");

            platform.transform.position = Vector3.MoveTowards(platform.transform.position, targetPosition, Time.deltaTime * dropSpeed);
            Debug.Log($"Moving to {targetPosition.y}");
            if (destination < 0)
            {
                if (platform.transform.position.y >= targetPosition.y)
                {
                    isCurrentlyAutoMoving = false;
                }
                else
                {
                    isCurrentlyAutoMoving = true;
                }
            }
            else
            {
                if (platform.transform.position.y <= targetPosition.y)
                {
                    isCurrentlyAutoMoving = false;
                }
                else
                {
                    isCurrentlyAutoMoving = true;
                }
            }
        }

    }

    public void MoveAutomated()
    {
            
            targetPosition = new Vector3(platform.transform.position.x, platform.transform.position.y + incrementAdded, platform.transform.position.z);

    }
    public void MoveManual()
    {
        Debug.Log("Is Manual");
        if (destination < 0)
        {
            platform.transform.Translate(new Vector3(0, -Time.deltaTime, 0));
            Debug.Log("Moving Negative");
        }
        else
        {
            platform.transform.Translate(new Vector3(0, Time.deltaTime, 0));
            Debug.Log("Moving Positive");
        }


    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Is Hit");
        if(isAutomated)
        {
            if(!isCurrentlyAutoMoving)
            {
                MoveAutomated();
                isCurrentlyAutoMoving = true;
            }
        }
        else
        {
            MoveManual();
            isCurrentlyAutoMoving = false;
        }
    }
}

