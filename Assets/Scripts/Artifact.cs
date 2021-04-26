using UnityEngine;

public class Artifact : MonoBehaviour
{
    private Rigidbody rigidBody;
    internal bool stashed;
    internal Vector3 homePosition;
    internal Quaternion homeRotation;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        homePosition = transform.position;
        homeRotation = transform.rotation;
    }

    internal void OnPickedUp(Transform hands)
    {
        // Use the kinematic approach to be picked up
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
        transform.SetParent(hands);
        
        // HACK: If the robot picked up the artifact
        if (hands.GetComponentInParent<Robot>())
        {
            // Reset the artifact's position
            transform.localPosition = Vector3.zero;
        }
    }

    internal void OnDropped()
    {
        // TODO: Make this work with the Throwable script

        // Use the kinematic approach to be dropped
        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
        transform.SetParent(null);
    }
}
