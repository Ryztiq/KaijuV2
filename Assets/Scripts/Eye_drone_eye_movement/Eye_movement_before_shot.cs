using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Eye_movement_before_shot : MonoBehaviour
{
    //{ .8f, 3, 0.4f, 3, 1 };
    private float[] eye_resize_speed = new float[] { .0f, 0.6f, -3.0f, 1.22f, -0.04f};

    //public GameObject core_for_emission;

    private int change_size = -1;

    private void Start()
    {
        //Invoke("Shoot_eye", 1f);
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
        Invoke("Fire", .5f);
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
    }

    private void Changing_size_of_eye_pupal()
    {
        //this.gameObject.transform.localScale = new Vector3(eye_resize_speed[change_size], eye_resize_speed[change_size], eye_resize_speed[change_size]);
        this.gameObject.transform.localScale += new Vector3(eye_resize_speed[change_size] * Time.deltaTime, eye_resize_speed[change_size] * Time.deltaTime, eye_resize_speed[change_size] * Time.deltaTime);
    }
}
