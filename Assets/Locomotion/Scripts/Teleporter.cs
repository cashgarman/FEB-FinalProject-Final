using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public string thumbstickInputName;
    public string turningInputName;
    public float thumbstickThreshold = -0.5f;
    public LineRenderer beam;
    public float range;
    public Color validColour;
    public Color invalidColour;
    public GameObject teleportIndicator;
    public Transform player;

    private bool hasValidTeleportTarget;
    private bool turning;
    public float turnInputThreshold;
    public float turnAngle;
    private Vector3 targetPosition;
    private bool moving;
    private float stepCountdown;
    public float walkSpeed;
    public float stoppingDistance;
    public float stepDelay;
    private float stepSize;

    void Start()
    {
        // Hide the beam intially
        SetBeamVisible(false);

        // Calculate the step size
        stepSize = walkSpeed * stepDelay;
    }

    void Update()
    {
        // If the thumbstick is pressed forward
        if(Input.GetAxis(thumbstickInputName) < thumbstickThreshold)
        {
            // Show the teleport beam
            SetBeamVisible(true);

            // Extend the beam out to its maximum range
            SetBeamEndPoint(transform.position + transform.forward * range);

            // Check if the beam hit something
            if(Physics.Raycast(transform.position, transform.forward, out var hit, range))
            {
                // Update the beam's endpoint to the point in space it hit
                SetBeamEndPoint(hit.point);

                // If the object we hit is a valid teleport target
                if(IsValidTeleportTarget(hit.collider.gameObject))
                {
                    // Set the beam to be valid
                    SetTeleportValid(true);

                    // Set the position of the teleport indicator
                    teleportIndicator.transform.position = hit.point + Vector3.up * 0.001f;
                }
                // If the object we hit is an invalid teleport target
                else
                {
                    // Set the beam to be invalid
                    SetTeleportValid(false);
                }
            }
            // If we didn't hit anything
            else
            {
                // Set the beam to be invalid
                SetTeleportValid(false);
            }
        }
        // If the thumbstick has been released
        else
        {
            // Hide the teleport beam
            SetBeamVisible(false);

            // Do we have a valid teleport target
            if(hasValidTeleportTarget)
            {
                // Teleport the player there
                //player.position = teleportIndicator.transform.position;

                // Set the target position for the player
                targetPosition = teleportIndicator.transform.position;
                moving = true;
                stepCountdown = 0;

                // Reset the teleport
                SetTeleportValid(false);
            }
        }

        // If we're moving to a target position
        if(moving)
        {
            // Move the player towards the target position
            //player.position += targetDir * walkSpeed * Time.deltaTime;

            // Decrease the step countdown
            stepCountdown -= Time.deltaTime;

            // If the step countdown has run out
            if(stepCountdown <= 0)
            {
                // Reset the step countdown
                stepCountdown = stepDelay;

                // TODO: Figure out why the robot is reseting its DetectingPlayer state each step

                // If the player is closer than 1 step size from the target position
                if(Vector3.Distance(player.position, targetPosition) <= stepSize)
                {
                    // Move the player to the target position
                    player.position = targetPosition;
                    moving = false;
                }
                // Otherwise
                else
                {
                    // Calculate the normalized direction to the target position
                    var targetDir = (targetPosition - player.position).normalized;

                    // Move the player towards the target position
                    player.position += targetDir * stepSize;
                }
            }
        }

        // Handle snap turning
        var turnInput = Input.GetAxis(turningInputName);
        if(!turning && turnInput <= -turnInputThreshold)
        {
            player.Rotate(0, -turnAngle, 0);
            turning = true;
        }
        if (!turning && turnInput >= turnInputThreshold)
        {
            player.Rotate(0, turnAngle, 0);
            turning = true;
        }
        if(turnInput > -turnInputThreshold && turnInput < turnInputThreshold)
        {
            turning = false;
        }
    }

    private void SetTeleportValid(bool valid)
    {
        // Set the appropriate colour of the beam
        beam.material.color = valid ? validColour : invalidColour;

        // Show or hide the teleport indicator as appropriate
        teleportIndicator.SetActive(valid);

        // Remember whether or not we have a valid target
        hasValidTeleportTarget = valid;
    }

    private bool IsValidTeleportTarget(GameObject target)
    {
        return true;
    }

    private void SetBeamEndPoint(Vector3 endPoint)
    {
        // Set the start and end positions of the beam
        beam.SetPosition(0, transform.position);
        beam.SetPosition(1, endPoint);
    }

    private void SetBeamVisible(bool visible)
    {
        // Show or hide the beam as appropriate
        beam.enabled = visible;
    }
}
