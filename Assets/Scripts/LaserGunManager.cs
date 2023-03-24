using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LaserGunManager : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint;
    public BulletManager.BulletStats bulletStats;

    //references
    public XRBaseController leftController, rightController;
    public float hapticAmplitude = 0.2f;
    public float hapticDuration = 0.5f;

    public void Fire()
    {
        GameObject bulletFire = Instantiate(bullet, firePoint.position, firePoint.rotation);
        bulletFire.GetComponent<BulletManager>().bulletStats = new BulletManager.BulletStats(bulletStats);
        leftController.SendHapticImpulse(1, 0.1f);
    }

    public void SendHaptics(XRBaseController controller, float amp, float dur)
    {
        controller.SendHapticImpulse(amp, dur);
    }

    public void SendHaptics()
    {
        rightController.SendHapticImpulse(0.5f, 0.1f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
