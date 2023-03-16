using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string TransitionTo;

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet")) SceneManager.LoadScene(TransitionTo);
    }
}
