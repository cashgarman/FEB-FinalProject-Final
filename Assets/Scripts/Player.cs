using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public Transform hands;
    public float stunnedDuration;

    public Artifact heldArtifact;
    private float timeLeftStunned;
    private NavMeshAgent navAgent;
    [SerializeField] private Transform _head;
    private Grabber _artifactGrabber;

    public bool Stunned { get; set; }
    public Vector3 HeadPosition => _head.position;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    public void OnObjectGrabbed(GameObject grabbedObject, GameObject grabber)
    {
        // If the thing we grabbed is an artifact
        var artifact = grabbedObject.GetComponent<Artifact>();
        if(artifact != null)
        {
            // Store the artifact the player is carrying
            heldArtifact = artifact;
            
            heldArtifact.OnPickedUp(grabber.transform);
            _artifactGrabber = grabber.GetComponent<Grabber>();
            
            // Alert all nearby robots
            GameManager.AlertNearbyRobotsToPlayer();
        }
    }
    
    public void OnObjectDropped(GameObject droppedObject, GameObject grabber)
    {
        var artifact = droppedObject.GetComponent<Artifact>();
        if(artifact != null)
        {
            heldArtifact.OnDropped();
            heldArtifact = null;
            _artifactGrabber = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Entered trigger {other.name}");

        // If the player entered a safe zone and is hold an artifact
        if(other.gameObject.CompareTag("SafeZone") && heldArtifact != null)
        {
            // Flag the artifact as stashed
            heldArtifact.stashed = true;

            // Drop the artifact
            heldArtifact.OnDropped();
            heldArtifact = null;
            _artifactGrabber.ForceDrop();
            _artifactGrabber = null;

            // Play the artifact stashed sound
            SoundController.PlaySound(gameObject, "artifact_stashed");

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
        if (Stunned)
        {
            timeLeftStunned -= Time.deltaTime;
            if (timeLeftStunned <= 0f)
            {
                Stunned = false;
            }
        }
    }

    internal void OnStunned()
    {
        // Flag the player as stunned
        Stunned = true;

        // Drop the artifact if we're holding one
        _artifactGrabber?.ForceDrop();
        _artifactGrabber = null;
        heldArtifact?.OnDropped();
        heldArtifact = null;

        // Play the stunned sound
        SoundController.PlaySound(gameObject, "robot_stun");

        // Start the stunned countdown
        timeLeftStunned = stunnedDuration;
    }
}
