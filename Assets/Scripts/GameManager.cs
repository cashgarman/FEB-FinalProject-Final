using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public float _alertDistance;

    public static Player Player => instance.player;

    private void Awake()
    {
        // Are there any other game managers yet?
        if(instance != null)
        {
            // Error
            Debug.LogError("There was more than 1 GameManager");
        }
        else
        {
            instance = this;
        }
    }

    internal static void OnArtifactStashed()
    {
       // If all of the artifacts are stashed
       if(FindObjectsOfType<Artifact>().All(artifact => artifact.stashed))
       {
            // You win!
            GameOver();
       }
    }

    private static void GameOver()
    {
        // TODO: Add some fanfare!
        Debug.Log("You win :|");
    }
    
    public static void AlertNearbyRobotsToPlayer()
    {
        foreach (var robot in FindObjectsOfType<Robot>()
            .Where(robot => Vector3.Distance(robot.transform.position, Player.transform.position) <= instance._alertDistance))
        {
            Debug.Log($"Alerting {robot.name}");
            robot.OnPlayerDetected();
        }
    }
}
