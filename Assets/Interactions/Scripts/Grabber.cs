using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Grabber : MonoBehaviour
{
    public string gripInputName;
    public string triggerInputName;

    private Grabbable touchedObject;
    private Grabbable grabbedObject;

    public UnityEvent<GameObject> onObjectGrabbed;
    public UnityEvent<GameObject> onObjectDropped;

    void Update()
    {
        // If the grip button is pressed
        if (Input.GetButtonDown(gripInputName))
        {
            // Play the gripped animation
            GetComponent<Animator>().SetBool("Gripped", true);

            // If we're touching something
            if (touchedObject != null)
            {
                // Let the touched object know it has been grabbed
                touchedObject.OnGrab(this);

                // Store the new grabbed object
                grabbedObject = touchedObject;

                // Trigger the object picked up event
                onObjectGrabbed.Invoke(grabbedObject.gameObject);
            }
        }

        // If the grip button is released
        if (Input.GetButtonUp(gripInputName))
        {
            // Stop playing the gripped animation
            GetComponent<Animator>().SetBool("Gripped", false);

            // If we have a grabbed object
            if (grabbedObject != null)
            {
                // Let the grabbed object know it's been dropped
                grabbedObject.OnDrop();

                // Trigger the object picked up event
                onObjectDropped.Invoke(grabbedObject.gameObject);

                // Reset the grabbed object
                grabbedObject = null;
            }
        }

        // If the trigger button is pressed
        if (Input.GetButtonDown(triggerInputName))
        {
            // If we have a grabbed object
            if (grabbedObject != null)
            {
                // Let the grabbed object know it has been triggered
                grabbedObject.OnTriggerStart();
            }
        }

        // If the trigger button is released
        if (Input.GetButtonUp(triggerInputName))
        {
            // If we have a grabbed object
            if (grabbedObject != null)
            {
                // Let the grabbed object know it has stopped being triggered
                grabbedObject.OnTriggerEnd();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object we touched is a grabbable object
        Grabbable grabbable = other.GetComponent<Grabbable>();
        if(grabbable != null)
        {
            // Let the object know it was touched
            grabbable.OnTouched();

            // Store the currently touched object
            touchedObject = grabbable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object we stopped touching is a grabbable object
        Grabbable grabbable = other.GetComponent<Grabbable>();
        if (grabbable != null)
        {
            // Let the object know it is no longer being touched
            grabbable.OnUntouched();

            // Reset the currently touched object
            // TODO: This will need more work when we have lots of object close together
            touchedObject = null;
        }
    }
}
