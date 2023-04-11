using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Lowere_Platfrom : MonoBehaviour
{
    

    public GameObject lowering_access;

    public float distance_activation = 5;

    void Start()
    {
        lowering_access = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Get_activate_lowering();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Disactivate_lowering();
        }

        if (lowering_access != null)
        {
            if (Vector3.Distance(this.transform.position, lowering_access.transform.position) <= distance_activation)
            {
                Activate_lowering();
            }
        }

    }

    //gets a oject with this tag and when lcose by it activates
    public void Get_activate_lowering()
    {
        lowering_access = GameObject.FindGameObjectWithTag("lower_platform_cranks");
    }

    private void Activate_lowering()
    {
        lowering_access.GetComponent<Platform_lower>().Platform_lowers();
    }

    private void Disactivate_lowering()
    {
        lowering_access.GetComponent<Platform_lower>().Platform_stop_lowers();
    }
}
