using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lower_platform : MonoBehaviour
{
    private GameObject platform;

    private float fall_speed = 0.04f;

   // private float fall_drop = .4f;

    private bool down = false;

    private Vector3 orign;

    private float wait_time = 1f;

    void Start()
    {
        platform = GameObject.FindGameObjectWithTag("platform_get");
        orign = platform.transform.position;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            Turn_off_down();
        }

        if(Input.GetKeyDown(KeyCode.H) && down == false)
        {
            Turn_on_down();
            //Invoke("Turn_off_down", wait_time);
        }
        
        if(Input.GetKeyDown(KeyCode.J))
        {
            platform.transform.position = orign;
        }

        if (down == true)
        {
            Lower_platform_slow();
        }
    }

    public void Turn_on_down()
    {
        down = true;
    }

    private void Lower_platform_slow()
    {
        platform.transform.Translate(0, (fall_speed * Time.deltaTime), 0);
    }

    public void Turn_off_down()
    {
        down = false;
    }
    


}
