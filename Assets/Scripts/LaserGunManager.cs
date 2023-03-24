using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;

public class LaserGunManager : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint;
    public BulletManager.BulletStats bulletStats;
    public AudioSource audioSource;

    //references
    public XRBaseController leftController, rightController;
    public float hapticAmplitude = 0.2f;
    public float hapticDuration = 0.5f;

    public void Fire()
    {
        GameObject bulletFire = Instantiate(bullet, firePoint.position, firePoint.rotation);
        bulletFire.GetComponent<BulletManager>().bulletStats = new BulletManager.BulletStats(bulletStats);
        leftController.SendHapticImpulse(1, 0.1f);
        float random = Random.Range(0.9f, 1.1f);
        audioSource.pitch = random;
        audioSource.Play();
    }

    public void SendHaptics(XRBaseController controller, float amp, float dur)
    {
        controller.SendHapticImpulse(amp, dur);
    }

    public void SendHaptics()
    {
        rightController.SendHapticImpulse(0.5f, 0.1f);
    }

    private void OnGUI()
    {
        //a button that fires the gun
        if (GUI.Button(new Rect(10, 10, 100, 50), "Fire"))
        {
            Fire();
        }
    }
}
