using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeMeter : MonoBehaviour
{
    public Material ChargeMat;
    public float time;
    public float maxTime;   

    // Start is called before the first frame update
    void Start()
    {
        ChargeMat.SetFloat("_ChargeAmount", time / maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
