using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Robot : Agent
{
    public Transform headBone;
    public GameObject detectionIndicator;
    public TMP_Text detectionIndicatorText;
    public float detectionTime;
    public float fieldOfView;
    public float stoppedDistance;
    public float returnHomeTime;
    [SerializeField] private float _caughtPlayerDistance;
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private GameObject _detectionIndicator;
    [SerializeField] private float _sightRange;

    private Player player;
    private Animator animator;
    private NavMeshAgent navAgent;
    private float timeLeftUntilDetected;
    private Vector3 lastKnownPlayerLocation;
    private float timeLeftUntilReturnHome;
    private Vector3 homePosition;
    private bool _playerDetected;
    private Artifact _targetArtifact;
    private Artifact _heldArtifact;
    private Quaternion _initialRotation;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        // Start in the idle state
        GotoState(State.Idle);

        // Set the robot's home location
        homePosition = transform.position;
        
        // Store the player
        player = GameManager.Player;
        
        // Store the initial rotation
        _initialRotation = transform.rotation;
    }

    protected override void OnStateEntered(State state)
    {
        base.OnStateEntered(state);

        // Handle entering the new state
        switch(state)
        {
            case State.Idle:
                // Stop the walking animation
                animator.SetBool("Walking", false);
                break;
            case State.DetectingPlayer:
                // TODO: Have the robot look at the player with their head

                // Show the detection indicator above the robot's head
                detectionIndicator.SetActive(true);

                // Start the detection countdown
                timeLeftUntilDetected = detectionTime;
                break;
            case State.ChasingPlayer:
                // Play the walking animation
                animator.SetBool("Walking", true);
                break;
            case State.GrabbingArtifact:
                // Play the walking animation
                animator.SetBool("Walking", true);
                
                // Move to the target artifact
                navAgent.SetDestination(_targetArtifact.transform.position);
                break;
            case State.ReturningArtifact:
                // Play the walking animation
                animator.SetBool("Walking", true);
                
                // Move to the artifact's home
                navAgent.SetDestination(_heldArtifact.homePosition);
                break;
            case State.MoveToLastKnownPlayerPosition:
                // Play the walking animation
                animator.SetBool("Walking", true);

                // Move to the player's last known location
                navAgent.SetDestination(lastKnownPlayerLocation);
                break;
            case State.LookingForPlayer:
                // Stop the walking animation
                animator.SetBool("Walking", false);

                // Start the return home countdown
                timeLeftUntilReturnHome = returnHomeTime;
                break;
            case State.ReturningHome:
                // Play the walking animation
                animator.SetBool("Walking", true);

                // Move back home
                navAgent.SetDestination(homePosition);
                break;
            case State.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    protected override void OnStateLeft(State state)
    {
        base.OnStateLeft(state);

        // Handle leaving the previous state
        switch (state)
        {
            case State.Idle:
                break;
            case State.DetectingPlayer:
                // Hide the detection indicator above the robot's head
                detectionIndicator.SetActive(false);
                break;
            case State.ChasingPlayer:
                break;
            case State.GrabbingArtifact:
                break;
            case State.ReturningArtifact:
                break;
            case State.MoveToLastKnownPlayerPosition:
                break;
            case State.LookingForPlayer:
                break;
            case State.ReturningHome:
                break;
            case State.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void Update()
    {
        // If the robot is trying to return an artifact, and the player has dropped it
        if (_targetArtifact != null && !_targetArtifact.stashed && !ShouldChasePlayer() && currentState != State.GrabbingArtifact)
        {
            // Go to the grabbing artifact state
            GotoState(State.GrabbingArtifact);
        }

        // Handle updating the current state
        switch (currentState)
        {
            case State.Idle:
                IdleUpdate();
                break;
            case State.DetectingPlayer:
                DetectingPlayerUpdate();
                break;
            case State.ChasingPlayer:
                ChasingPlayerUpdate();
                break;
            case State.GrabbingArtifact:
                GrabbingArtifactUpdate();
                break;
            case State.ReturningArtifact:
                ReturningArtifactUpdate();
                break;
            case State.MoveToLastKnownPlayerPosition:
                MoveToLastKnownPlayerPositionUpdate();
                break;
            case State.LookingForPlayer:
                LookingForPlayerUpdate();
                break;
            case State.ReturningHome:
                ReturningHomeUpdate();
                break;
            case State.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null);
        }
    }

    private void IdleUpdate()
    {
        // Can the robot see the player and we haven't detected them
        if(CanSeePlayer() && !_playerDetected)
        {
            // Go to the detecting player state
            GotoState(State.DetectingPlayer);
            return;
        }
        
        // If we have detected the player and they should be chased
        if (_playerDetected && ShouldChasePlayer())
        {
            // Store the target artifact
            _targetArtifact = player.heldArtifact;
            
            // Go to the chase player state
            GotoState(State.ChasingPlayer);
        }
        
        // If we can't see the player and they were detected
        if (!CanSeePlayer() && _playerDetected)
        {
            // Undetect the player
            OnPlayerUndetected();
            
            // Return to face the initial direction
            transform.rotation = _initialRotation;
        }
        
        // If the player is detected and we can see them, face them
        if (_playerDetected && CanSeePlayer())
        {
            var lookDir = player.transform.position - transform.position;
            lookDir.y = 0;
            
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir), 0.5f);
        }
    }

    private bool CanSeePlayer()
    {
        // TODO: Adjust the raycast to aim from the robot's head to the player's head, to avoid low obstacles

        // Calculate the direction from the robot's head to the player
        var playerDirection = (player.transform.position - headBone.position).normalized;

        if (Physics.Raycast(headBone.position, playerDirection, out var hit, _sightRange))
        {
            // Is the player within the robot's field of view
            var angleToPlayer = Vector3.Angle(transform.forward, playerDirection);
            if(angleToPlayer >= -(fieldOfView / 2f) && angleToPlayer <= (fieldOfView / 2f))
            {
                // If the player was the thing we hit
                return hit.collider.gameObject.CompareTag("Player");
            }
        }

        return false;
    }

    private bool ShouldChasePlayer()
    {
        // Chase the player if they are holding an artifact
        return player.heldArtifact != null;
    }

    private void DetectingPlayerUpdate()
    {
        // Countdown the detection time
        timeLeftUntilDetected -= Time.deltaTime;

        // Update detection indicator
        var percent = 1f - (timeLeftUntilDetected / detectionTime);
        detectionIndicatorText.text = $"{percent * 100f:F0}%";

        // If the player has been fully detected
        if(timeLeftUntilDetected <= 0)
        {
            // Flag the player as detected
            OnPlayerDetected();
            
            // If we should chase the player
            if (ShouldChasePlayer())
            {
                // Store the target artifact
                _targetArtifact = player.heldArtifact;
                
                // Goto the chasing state
                GotoState(State.ChasingPlayer);
            }
            // Otherwise
            else
            {
                // Go back to the idle state
                GotoState(State.Idle);
            }
        }

        // If the robot lost sight of the player
        if(!CanSeePlayer())
        {
            // Go back to the idle state
            GotoState(State.Idle);
        }
    }

    public void OnPlayerDetected()
    {
        _playerDetected = true;
        
        // Show the detection indicator
        _detectionIndicator.SetActive(true);
        
        // Snap the robot to face the player
        var lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }

    private void OnPlayerUndetected()
    {
        _playerDetected = false;
        
        // Hide the detection indicator
        _detectionIndicator.SetActive(false);
    }

    private void ChasingPlayerUpdate()
    {
        // Set the robot's destination to the player's position
        navAgent.SetDestination(player.transform.position);

        // If the player has stashed the artifact
        if (_targetArtifact != null && _targetArtifact.stashed)
        {
            // Give up and go home
            _targetArtifact = null;
            GotoState(State.ReturningHome);
            return;
        }
        
        // If the player has dropped the artifact
        if (!ShouldChasePlayer())
        {
            // Go the grab artifact state
            GotoState(State.GrabbingArtifact);
            return;
        }
        
        // If the robot gets close enough to the player
        if (Vector3.Distance(player.transform.position, transform.position) <= _caughtPlayerDistance)
        {
            // Stun the player
            player.OnStunned();
            
            // Goto the grab artifact state
            GotoState(State.GrabbingArtifact);
            return;
        }

        // If the robot loses sight of the player
        if(!CanSeePlayer())
        {
            // Store the last known location of the player
            lastKnownPlayerLocation = player.transform.position;

            // Go to the player's last known location state
            GotoState(State.MoveToLastKnownPlayerPosition);
            return;
        }
    }

    private void GrabbingArtifactUpdate()
    {
        // Move to the target artifact
        navAgent.SetDestination(_targetArtifact.transform.position);
        
        // If the player has stashed the artifact
        if (_targetArtifact != null && _targetArtifact.stashed)
        {
            // Give up and go home
            _targetArtifact = null;
            GotoState(State.ReturningHome);
            return;
        }

        // Has the player picked up the artifact again
        if (ShouldChasePlayer())
        {
            // Go to the chasing player state
            GotoState(State.ChasingPlayer);
            return;
        }
    }

    private void ReturningArtifactUpdate()
    {
        // If the robot has reached the artifact's home
        if (navAgent.remainingDistance <= stoppedDistance)
        {
            // Drop the artifact
            _heldArtifact.OnDropped();
            _heldArtifact = null;
            
            // Go to the return home start
            GotoState(State.ReturningHome);
            return;
        }
    }

    private void MoveToLastKnownPlayerPositionUpdate()
    {
        // If we reached the player's last known position
        if(navAgent.remainingDistance <= stoppedDistance)
        {
            // Go to the looking around for player state
            GotoState(State.LookingForPlayer);
            return;
        }

        // If the player was spotted again
        if(CanSeePlayer())
        {
            // Resume the chase
            GotoState(State.ChasingPlayer);
            return;
        }
        
        // If the player has stashed the artifact
        if (_targetArtifact != null && _targetArtifact.stashed)
        {
            // Give up and go home
            _targetArtifact = null;
            GotoState(State.ReturningHome);
            return;
        }
    }

    private void LookingForPlayerUpdate()
    {
        // Countdown the return home time
        timeLeftUntilReturnHome -= Time.deltaTime;

        // If the robot is totally bored now
        if (timeLeftUntilReturnHome <= 0)
        {
            // Goto the chasing state
            GotoState(State.ReturningHome);
            return;
        }

        // If the robot sees the player again
        if (CanSeePlayer() && ShouldChasePlayer())
        {
            // Resume the chase
            GotoState(State.ChasingPlayer);
            return;
        }
        
        // If the player has stashed the artifact
        if (_targetArtifact != null && _targetArtifact.stashed)
        {
            // Give up and go home
            _targetArtifact = null;
            GotoState(State.ReturningHome);
            return;
        }
    }

    private void ReturningHomeUpdate()
    {
        // If we reached the robot's home position
        if (navAgent.remainingDistance <= stoppedDistance)
        {
            // Go to idle state
            GotoState(State.Idle);
            return;
        }

        // If the player was spotted again
        if (CanSeePlayer() && ShouldChasePlayer())
        {
            // Resume the chase
            GotoState(State.ChasingPlayer);
            return;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // If we're in the grabbing artifact state
        if (currentState == State.GrabbingArtifact)
        {
            // If the collided object is an artifact
            var artifact = other.collider.GetComponent<Artifact>();
            if (artifact == _targetArtifact)
            {
                // Grab the artifact
                artifact.OnPickedUp(_holdPoint);
                _heldArtifact = artifact;
                _targetArtifact = null;
                
                // Go to the return artifact home state
                GotoState(State.ReturningArtifact);
                return;
            }
        }
    }
}
