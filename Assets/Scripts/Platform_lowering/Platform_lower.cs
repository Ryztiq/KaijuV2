using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_lower : MonoBehaviour
{

    public GameObject[] objects_and_platform_lowering;

    public bool platform_lowering = false;

    public float speed_of_lowering = 0.1f;

    private float orig_point;

    public float distance_from_origin_lose = 5;

    // Start is called before the first frame update
    void Start()
    {
        if(objects_and_platform_lowering[0] != null)
        {
            orig_point = objects_and_platform_lowering[0].transform.position.y;
        }
        else
        {
            Debug.Log("You have no ojbects in the objects_and_platform_lowering list");
        }
                     
    }

    // Update is called once per frame
    void Update()
    {
        if (platform_lowering == true)
        {
            Platform_translation();
        }

        Lose_even_activation();
    }

    private void Lose_even_activation()
    {
        if((orig_point - distance_from_origin_lose) >= objects_and_platform_lowering[0].transform.position.y)
        {
            Debug.Log("Lose event?");
        }
    }

    public void Platform_lowers()
    {
        platform_lowering = true;
    }

    public void Platform_stop_lowers()
    {
        platform_lowering = false;        
    }

    private void Platform_translation()
    {
        
        foreach (GameObject obj in objects_and_platform_lowering)
        {
            obj.transform.Translate(0, -speed_of_lowering * Time.deltaTime, 0);
        }
    }

}
