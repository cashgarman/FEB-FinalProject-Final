using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
    private HingeJoint hinge;
    public float onAngleThreshold;
    public float offAngleThreshold;
    private bool pushedForward;
    private bool pulledBackward;

    public UnityEvent turnedOn;
    public UnityEvent turnedOff;

    private void Start()
    {
        hinge = GetComponent<HingeJoint>();
    }

    void Update()
    {
        // If the lever has been fully pushed forward (and it wasn't already)
        if(hinge.angle > onAngleThreshold && !pushedForward)
        {
            pushedForward = true;

            // Trigger the turned on event
            turnedOn.Invoke();
        }
        // If the lever was pushed forward and isn't anymore
        if(hinge.angle < onAngleThreshold)
        {
            pushedForward = false;
        }

        // If the lever has been fully pulled backward (and it wasn't already)
        if (hinge.angle < offAngleThreshold && !pulledBackward)
        {
            pulledBackward = true;

            // Trigger the turned off event
            turnedOff.Invoke();
        }
        // If the lever was pulled backward and isn't anymore
        if (hinge.angle > offAngleThreshold)
        {
            pulledBackward = false;
        }
    }
}
