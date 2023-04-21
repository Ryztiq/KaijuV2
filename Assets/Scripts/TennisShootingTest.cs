using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisShootingTest : MonoBehaviour
{

    public GameObject Bullet;

    // Start is called before the first frame update
    void Start()
    {
        ShootEvery5();
    }

    // Update is called once per frame
    void Update()
    {
       
    }



    private void ShootEvery5()
    {
        Instantiate(Bullet, transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        Invoke("ShootEvery5", .5f);
    }


}
