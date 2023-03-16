using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugShootScript : MonoBehaviour
{
    public GameObject bullet;
    private Camera camera;
    public BulletManager.BulletStats bulletStats; 

    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject spawnObject = Instantiate(bullet, camera.transform.position, GetAimDirection()); // spawn the object at the mouse click position with the correct rotation
            spawnObject.GetComponent<BulletManager>().bulletStats = bulletStats; // set the bullet stats
        }
    }

    public Quaternion GetAimDirection()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Input.mousePosition.x,Input.mousePosition.y,1)); // cast a ray from the camera to where the mouse clicked
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 clickPosition = hit.point; // get the position of the mouse click in the scene
            Vector3 cameraPosition = camera.transform.position; // get the position of the camera
            Vector3 direction = clickPosition - cameraPosition; // get the direction vector from the camera to the mouse click position
            Quaternion rotation = Quaternion.LookRotation(direction); // get the rotation to look at the mouse click position
            Debug.DrawRay(camera.transform.position, direction, Color.red); // draw a debug ray to see where the raycast is going
            return rotation;
        }
        else return quaternion.identity;
    }
}
