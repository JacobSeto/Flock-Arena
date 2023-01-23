using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnManager : MonoBehaviourPunCallbacks
{

    PhotonView view;
    private float lastSpawnIndex = -1;

    Spawnpoint[] spawnpoints;
    public static SpawnManager Instance;
    private void Awake()
    {
        view = GetComponent<PhotonView>();
        Instance = this;
        spawnpoints = GetComponentsInChildren<Spawnpoint>();
    }

    public Transform GetSpawnpoint()
    {
        //returns transform of random spawnpoint.  Tracks last spawn so players cannot repeat spawn index
        int ranSpawn = Random.Range(0, spawnpoints.Length);
        while(ranSpawn == lastSpawnIndex)
        {
            ranSpawn = Random.Range(0, spawnpoints.Length);
        }
        view.RPC(nameof(RPC_UpdateLastSpawnIndex), RpcTarget.All, ranSpawn);
        return spawnpoints[ranSpawn].transform;
    }

    [PunRPC]
    public void RPC_UpdateLastSpawnIndex(int index)
    {
        lastSpawnIndex = index;
    }
}
