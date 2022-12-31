using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private float lastSpawnIndex = -1;

    Spawnpoint[] spawnpoints;

    private void Awake()
    {
        Instance = this;
        spawnpoints = GetComponentsInChildren<Spawnpoint>();
    }

    public Transform GetSpawnpoint()
    {
        int ranSpawn = Random.Range(0, spawnpoints.Length);
        while(ranSpawn == lastSpawnIndex)
        {
            ranSpawn = Random.Range(0, spawnpoints.Length);
        }
        return spawnpoints[ranSpawn].transform;
    }
}
