using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LaserGunManager : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint;
    
    //references
    public XRBaseController leftController, rightController;
    public float hapticAmplitude = 0.2f;
    public float hapticDuration = 0.5f;

    public void Fire()
    {
        Instantiate(bullet, firePoint.position, firePoint.rotation);
    }

    public void SendHaptics(XRBaseController controller, float amp, float dur)
    {
        controller.SendHapticImpulse(amp, dur);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
