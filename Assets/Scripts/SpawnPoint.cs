using System;
using UnityEngine;

public enum SpawnType
{
    Artifact,
    Robot,
    Player
}

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private SpawnType _type;
    [SerializeField] private GameObject _artifactPrefab;
    [SerializeField] private GameObject _robotPrefab;

    public SpawnType Type => _type;
    
    void Start()
    {
        // Spawn the appropriate thing
        switch (_type)
        {
            case SpawnType.Artifact:
            {
                Instantiate(_artifactPrefab, transform.position, transform.rotation);
                Debug.Log($"Spawned an artifact at {transform.position}");
                break;
            }
            case SpawnType.Robot:
            {
                Instantiate(_robotPrefab, transform.position, transform.rotation);
                Debug.Log($"Spawned a robot at {transform.position}");
                break;
            }
            case SpawnType.Player:
            {
                // Ignore this here, this is handled by the GameManager
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}