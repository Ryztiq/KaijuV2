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
        //platform.transform.position = Vector3.Lerp(platform.transform.position, targetPosition, dropSpeed * Time.deltaTime);
        if (!isCurrentlyAutoMoving)
        {
            isCurrentlyAutoMoving = true;
            targetPosition = new Vector3(platform.transform.position.x, platform.transform.position.y + incrementAdded, platform.transform.position.z);
            StartCoroutine(MoveOverSpeed(platform, targetPosition, dropSpeed * Time.deltaTime));

        }

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
            }
        }
        else
        {
            MoveManual();
            isCurrentlyAutoMoving = false;
        }
    }

    //StartCoroutine(MoveOverSeconds (gameObject, new Vector3(0.0f, 10f, 0f), 5f));

    //https://answers.unity.com/questions/572851/way-to-move-object-over-time.html
    public IEnumerator MoveOverSpeed(GameObject objectToMove, Vector3 end, float speed)
    {
        // speed should be 1 unit per second
        while (objectToMove.transform.position != end)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
        isCurrentlyAutoMoving = false;
    }
}

