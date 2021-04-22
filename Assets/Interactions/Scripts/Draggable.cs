using UnityEngine;

public class Draggable : Grabbable
{
    public float springForce = 100f;
    public float dampingForce = 1f;

    private SpringJoint joint;

    public override void OnGrab(Grabber grabber)
    {
        // Add a spring joint between this object's rigid body and the grabber's rigid body
        joint = gameObject.AddComponent<SpringJoint>();
        joint.spring = springForce;
        joint.damper = dampingForce;
        joint.connectedBody = grabber.GetComponent<Rigidbody>();
    }

    public override void OnDrop()
    {
        // Remove the spring joint
        Destroy(joint);
    }
}
