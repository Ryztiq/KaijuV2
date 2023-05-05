using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;

public class WindowBreakinator : MonoBehaviour
{
    public GameObject shutter;

    [SerializeField]private List<WindowPiece> windowPieceScripts = new List<WindowPiece>();

    public Rigidbody rb;
    public MeshRenderer mesh;

    public GameObject pushPoint;
    public float velocityToBreak = 10;
    public AudioSource audioSource;


    [Serializable]
    public class WindowPiece
    {
        [FormerlySerializedAs("piece")] public GameObject gameObj;
        public Rigidbody rb;
        public Collider col;
        
        public WindowPiece(GameObject obj)
        {
            gameObj = obj;
            rb = obj.GetComponent<Rigidbody>();
            col = obj.GetComponent<Collider>();
        }
    }
    

    
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            windowPieceScripts.Add(new WindowPiece(child.gameObject));
        }
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DroneBullet"))
        {
            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
            float percent = Mathf.Clamp01(otherRb.velocity.magnitude/ velocityToBreak)*100;
            print($"{collision.gameObject.name} collider velocity: {percent.ToString(CultureInfo.InvariantCulture)}%");
            if (!(otherRb.velocity.magnitude > velocityToBreak)) return;
            //no damage hit
            if (mesh.enabled)
            {
                //crack the window by turning off the mesh for the unbroken window and turning on the meshes for the broken window pieces
                mesh.enabled = false;
                foreach (var piece in windowPieceScripts) piece.gameObj.SetActive(true);
            }
            //if the window has been hit before
            else
            {
                //break window
                foreach (var piece in windowPieceScripts)
                {
                    piece.rb.isKinematic = false;
                    piece.col.isTrigger = false;
                    var pushPosition = pushPoint.transform.position;
                    piece.rb.AddForceAtPosition((transform.position - pushPosition).normalized*100, pushPosition);
                    LifeTimeDespawn despawn = piece.gameObj.AddComponent<LifeTimeDespawn>();
                    despawn.LastingTime = 10;
                    despawn.waitForRB = false;
                }
                StartCoroutine(CloseShutter());
            }
        }
    }

    public IEnumerator CloseShutter()
    {
        yield return new WaitForSeconds(1);
        shutter.SetActive(true);
        LeanTween.moveLocalZ(shutter, 0, 2).setEaseOutBounce();
    }
}
