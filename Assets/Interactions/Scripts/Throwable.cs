using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Throwable : Grabbable
{
    public int numVelocitySamples = 10;
    public float throwBoost = 1f;

    private FixedJoint joint;
    private Vector3 prevPosition;
    private Queue<Vector3> previousVelocities = new Queue<Vector3>();

    public override void OnGrab(Grabber grabber)
    {
        // Add a fixed joint between this object's rigid body and the grabber's rigid body
        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = grabber.GetComponent<Rigidbody>();
    }

    public override void OnDrop()
    {
        // Remove the fixed joint
        Destroy(joint);

        // Calculate the average velocity from all the velocity samples
        Vector3 averageVelocity = Vector3.zero;
        foreach(Vector3 velocity in previousVelocities)
        {
            averageVelocity += velocity;
        }
        averageVelocity /= previousVelocities.Count;

        // Apply the calculated averaged velocity to the rigid body to throw it (with an optional boost)
        GetComponent<Rigidbody>().velocity = averageVelocity * throwBoost;
    }

    private void Update()
    {
        // Calculate the velocity of the object since the last update
        Vector3 velocity = transform.position - prevPosition;

        // Remember the position from this update so we can use it to calculate the velocity in the next update
        prevPosition = transform.position;

        // Add this calculated velocity to the list of previous velocities
        // TODO: This should probably be time-based, so it would be framerate-independent
        previousVelocities.Enqueue(velocity);

        // Make sure we don't store too many previous velocity samples
        if(previousVelocities.Count > numVelocitySamples)
        {
            // Toss out the oldest sample
            previousVelocities.Dequeue();
        }
    }
}
