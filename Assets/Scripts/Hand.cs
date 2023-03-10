using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class Hand : XRDirectInteractor
{
    public SkinnedMeshRenderer meshRenderer = null;
    protected override void Awake()
    {
        base.Awake();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public void Setvisibility(bool value)
    {
        meshRenderer.enabled = value;
    }
}
