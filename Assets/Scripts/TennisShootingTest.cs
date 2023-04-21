using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisShootingTest : MonoBehaviour
{

    public GameObject bulletPrefab;
    public BulletManager.BulletStats bullet;
    public AudioSource droneAudio;
    public AudioClip sfx;


    // Start is called before the first frame update
    void Start()
    {
        ShootEvery5();
    }

    private void Shoot()
    {
        droneAudio.pitch = Random.Range(0.9f, 1.1f);
        droneAudio.PlayOneShot(sfx);
        droneAudio.pitch = 1;
        GameObject spawnObject = Instantiate(bulletPrefab, transform.position, transform.rotation); // spawn the object at the mouse click position with the correct rotation
        //create a copy of bulletstats and assign it to the bulletstats of spawnobject
        spawnObject.GetComponent<BulletManager>().bulletStats = new BulletManager.BulletStats(bullet);
    }

    private void ShootEvery5()
    {
        Shoot();
        Invoke("ShootEvery5", .5f);
    }


}
