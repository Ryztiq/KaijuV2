using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveButton : MonoBehaviour
{
    public float speed = 5f;
    public float distance = 0.1f;
    
    public List<GameObject> PersonTargets = new List<GameObject>();
    private List<Vector3> PersonTargetPositions = new List<Vector3>();
    public GameObject PersonTargetPrefab;

    private Vector3 originalPosition;

    private bool isMovingDown = false;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        foreach (var Obj in PersonTargets)
        {
            PersonTargetPositions.Add(Obj.transform.position);
        }
    }

    public void ResetPersonTargets()
    {
        if(PersonTargets.Count >0) foreach (var target in PersonTargets) Destroy(target);
        
        for (int i = 0; i < PersonTargetPositions.Count; i++)
        {
            Instantiate(PersonTargetPrefab, PersonTargetPositions[i], Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
       

        if (isMovingDown)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, originalPosition - new Vector3(0f, distance, 0f), step);
            if (transform.position == originalPosition - new Vector3(0f, distance, 0f))
            {
                isMovingDown = false;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        print($"SOMETHING ENTERED: {other.gameObject.name}");
        if (other.gameObject.CompareTag("Bullet")) if(!LeanTween.isTweening(gameObject))ButtonAnimation();
    }

   public void IsMoving()
    {
        isMovingDown = true;
    }
   
   public void ButtonAnimation()
    {
        LeanTween.moveY(gameObject, transform.position.y - 0.06f, 0.1f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(1).setOnComplete(() => ResetPersonTargets());
    }

    public void ResetPosition()
    {
        StartCoroutine(Delay());
        transform.position = originalPosition;
        isMovingDown = false;
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2);
    }
   
}
