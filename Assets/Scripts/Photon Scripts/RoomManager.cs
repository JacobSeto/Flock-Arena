using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    [SerializeField] List<string> gamemodeNames;  //the list of game modes player can choose from.  Their names are used to load the correct scene

    private void Awake()
    {
        if (Instance) //checks if another RoomManager exists
        {
            Destroy(gameObject);  // destroys itself so there can only be one
            return;
        }
        DontDestroyOnLoad(gameObject);  // This script stays as the only one in the game
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (gamemodeNames.Contains(scene.name))  // in game scene
        {
            PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Player Manager"), Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.Log("scene not in gamemode list");
        }
    }
}

