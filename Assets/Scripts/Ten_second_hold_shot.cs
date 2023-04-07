using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ten_second_hold_shot : MonoBehaviour
{
    //{ .8f, 3, 0.4f, 3, 1 };
    private float[] eye_resize_speed = new float[] { .0f, 0.6f, -3.0f, 1.22f, -0.04f, -0.06f, 0.06f };



    private float wait_every_ten = 7;
    private float burst_time = 1;
    private float burst_time_intake = 0;

    //public GameObject core_for_emission;

    private int change_size = -1;

    private void Start()
    {
        Invoke("Shoot_eye", .01f);
    }

    void Update()
    {
        if (change_size > -1 && change_size < eye_resize_speed.Length)
        {
            Changing_size_of_eye_pupal();
        }

        //for testing purposes
        /*if (Input.GetKeyDown(KeyCode.G))
        {
            Shoot_eye();
        }*/
    }

    public void Shoot_eye()
    {
        change_size = 1;
        Invoke("Charge", 1.5f);
    }

    private void Charge()
    {
        change_size = 2;
        Invoke("Anticipate", .5f);
    }

    private void Anticipate()
    {
        change_size = 3;
        if (burst_time > 0)
        {
            burst_time_intake = burst_time;

            Invoke("Burst_down", .5f);
        }
        else
        {
            Invoke("Fire", .5f);
        }
    }

    private void Burst_down()
    {
        change_size = 5;
        burst_time_intake = burst_time_intake - 0.1f;
        Invoke("Burst_up", .1f);
    }

    private void Burst_up()
    {
        change_size = 6;
        burst_time_intake = burst_time_intake - 0.1f;
        if (burst_time_intake > 0)
        {
           Invoke("Burst_down", .1f);
        }
        else
        {
            Invoke("Fire", .1f);
        }
    }

    private void Fire()
    {
        change_size = 4;
        Invoke("Return_to_normal", .5f);
    }


    private void Return_to_normal()
    {
        change_size = -1;
        this.gameObject.transform.localScale = new Vector3(.8f, .8f, .8f);
        Invoke("Shoot_eye", wait_every_ten);
    }

    private void Changing_size_of_eye_pupal()
    {
        //this.gameObject.transform.localScale = new Vector3(eye_resize_speed[change_size], eye_resize_speed[change_size], eye_resize_speed[change_size]);
        this.gameObject.transform.localScale += new Vector3(eye_resize_speed[change_size] * Time.deltaTime, eye_resize_speed[change_size] * Time.deltaTime, eye_resize_speed[change_size] * Time.deltaTime);
    }
}
