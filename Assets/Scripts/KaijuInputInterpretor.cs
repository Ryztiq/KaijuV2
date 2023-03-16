using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class KaijuInputInterpretor : MonoBehaviour
{
    public Hand leftHand;
    public Hand rightHand;
    public Transform head;

    [Serializable]
    public class Hand
    {
        public ActionBasedController controller;
        public XRDirectInteractor interactor;
        [FormerlySerializedAs("trigger")] public InputActionReference triggerValue;
        public InputActionReference triggerPress;
        [FormerlySerializedAs("grab")] public InputActionReference grabValue;
        public InputActionReference grabPress;
        public InputActionReference trackState;
        public Transform transform;
        public InputActionReference haptics;
        public InputActionReference uiButton;
        public InputActionReference primaryStick;
        public InputActionReference turn;
    }

    private void Update()
    {
        
    }

    public void OnGUI()
    {
        //text with a value from 0-100 controlled by the right hand trigger input
        
    }
}
