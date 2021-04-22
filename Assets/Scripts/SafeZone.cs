using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"{other.name} entered the safe zone");

        // If the player entered a safe zone and is hold an artifact
        /*if (other.gameObject.CompareTag("SafeZone") && heldArtifact != null)
        {
            Debug.Log($"reached");

            // Flag the artifact as stashed
            heldArtifact.stashed = true;

            // Drop the artifact
            heldArtifact.OnDropped();
            heldArtifact = null;

            // Let the game know an artifact was stashed
            GameManager.OnArtifactStashed();
        }*/
    }
}
