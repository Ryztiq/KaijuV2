using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SendController : MonoBehaviour
{
    public XRBaseController controller;
    public XRBaseInteractor interactor;
    public IXRSelectInteractable interactable;

    public void SendInfo()
    {

    }

    public void SetInteractable()
    {
        interactable = interactor.firstInteractableSelected;
        print(interactable);
    }

}
