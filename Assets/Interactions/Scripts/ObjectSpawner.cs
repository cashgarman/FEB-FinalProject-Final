using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public float spawnInterval;
    public GameObject objectPrefab;

    private float spawnCountdown;

    private void Start()
    {
        spawnCountdown = spawnInterval;
    }

    void Update()
    {
        spawnCountdown -= Time.deltaTime;     
        if(spawnCountdown <= 0f)
        {
            SpawnObject();
            spawnCountdown = spawnInterval;
        }
    }

    private void SpawnObject()
    {
        Debug.Log($"Spawning a {objectPrefab.name}");

        Instantiate(objectPrefab, transform.position, Quaternion.identity);
    }
}
