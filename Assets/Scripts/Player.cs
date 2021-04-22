using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public Transform hands;
    public float stunnedDuration;

    public Artifact heldArtifact;
    private bool stunned;
    private float timeLeftStunned;
    private NavMeshAgent navAgent;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        // If the thing we touched is an artifact
        var artifact = collision.collider.GetComponent<Artifact>();
        if(artifact != null && !artifact.stashed && !stunned)
        {
            // Pickup the artifact
            artifact.OnPickedUp(hands);

            // Store the artifact the player is carrying
            heldArtifact = artifact;

            // Alert all nearby robots
            GameManager.AlertNearbyRobotsToPlayer();
        }
    }*/

    public void OnObjectGrabbed(GameObject grabbedObject)
    {
        // If the thing we grabbed is an artifact
        var artifact = grabbedObject.GetComponent<Artifact>();
        if(artifact != null)
        {
            // Store the artifact the player is carrying
            heldArtifact = artifact;
            
            // Alert all nearby robots
            GameManager.AlertNearbyRobotsToPlayer();
        }
    }
    
    public void OnObjectDropped(GameObject droppedObject)
    {
        var artifact = droppedObject.GetComponent<Artifact>();
        if(artifact != null)
        {
            heldArtifact = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: Get the SafeZone detecting here

        Debug.Log($"Entered trigger {other.name}");

        // If the player entered a safe zone and is hold an artifact
        if(other.gameObject.CompareTag("SafeZone") && heldArtifact != null)
        {
            Debug.Log($"reached");

            // Flag the artifact as stashed
            heldArtifact.stashed = true;

            // Drop the artifact
            heldArtifact.OnDropped();
            heldArtifact = null;

            // Let the game know an artifact was stashed
            GameManager.OnArtifactStashed();
        }
    }

    private void Update()
    {
        // Drop the artifact when space is pressed and we're holding an artifact
        if(Input.GetKeyDown(KeyCode.Space) && heldArtifact != null)
        {
            // Drop the artifact
            heldArtifact.OnDropped();
            heldArtifact = null;

            // TODO: Throw the artifact?
        }

        // If we're stunned
        if (stunned)
        {
            timeLeftStunned -= Time.deltaTime;
            if (timeLeftStunned <= 0f)
            {
                stunned = false;
            }

            // TODO: Make this work again with VR

            // Prevent the player from moving
            //navAgent.ResetPath();
            //navAgent.isStopped = true;
        }
        else
        {
            //navAgent.isStopped = false;
        }
    }

    internal void OnStunned()
    {
        // Flag the player as stunned
        stunned = true;

        // Drop the artifact if we're holding one
        heldArtifact?.OnDropped();
        heldArtifact = null;

        // Start the stunned countdown
        timeLeftStunned = stunnedDuration;
    }
}
