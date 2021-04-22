using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebugPlayerControls : MonoBehaviour
{
    private NavMeshAgent navAgent;

    void Start()
    {
        // Store the nav mesh agent
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // If the left mouse button has been pressed
        if(Input.GetMouseButtonDown(0))
        {
            // Convert the mouse screen position to a world ray
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast into the world
            if(Physics.Raycast(ray, out var hit))
            {
                // If the object that was clicked is "Walkable"
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Walkable"))
                {
                    // Set the player's destination to the point we clicked
                    navAgent.SetDestination(hit.point);
                }
            }
        }
    }
}
