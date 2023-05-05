using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ShieldDestroyer : MonoBehaviour
{
    public GameObject shield;
    public DroneController drone;

    public void DestroyShield()
    {
        Destroy(shield);
    }

    public void GeneratorDestroyed()
    {
        drone.droneAudio.PlayOneShot(drone.sfx[0]);
        if(drone.phase != 2) drone.shield.SpawnShield();
        drone.invincibleShield.DisableShield();
        drone.smallBullet = true;
        drone.StartCoroutine(drone.PauseAttack(3));
        drone.phase++;
        drone.shieldUp = false;
    }
}
