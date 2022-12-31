using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    PhotonView view;
    GameObject player;
    [SerializeField] PlayerLoadout playerLoadout;
    [SerializeField] GameObject playerLoadoutUI;
    [SerializeField] GameObject playerLoadoutCamera;

    int kills;
    int deaths;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        playerLoadout.SetTeam("blue");
        SetPlayerLoadout();
    }

    public void SetPlayerLoadout()
    {
        if (view.IsMine)
        {
            playerLoadoutUI.SetActive(true);
            playerLoadoutCamera.SetActive(true);
        }
        else
        {
            Destroy(playerLoadoutCamera);
            Destroy(playerLoadoutUI);
        }
    }

    public void CreatePlayer()
    {
        if (!view.IsMine)
            return;
        playerLoadoutCamera.SetActive(false);
        //When Loadout set, create the player controller
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        //Instantiate the player controller
        player = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Player Controller"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { view.ViewID});
        Debug.Log("Instantiated Player Controller");      
        playerLoadoutUI.SetActive(false);
    }

    public PlayerLoadout GetPlayerLoadout()
    {
        return playerLoadout;
    }

    public void Die()
    {
        PhotonNetwork.Destroy(player);
        SetPlayerLoadout();

        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill()
    {
        view.RPC(nameof(RPC_GetKill), view.Owner);
    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.view.Owner == player);
    }
}
