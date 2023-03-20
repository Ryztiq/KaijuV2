using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URPDemoScript : MonoBehaviour
{
    public Renderer rend;

    public float fill=0;

    private static readonly int FillAmount = Shader.PropertyToID("_FillAmount");

    // Update is called once per frame
    void Update()
    {
        fill = (Mathf.Sin(Time.time) +1) /2;
        rend.material.SetFloat(FillAmount, fill);
    }
}
