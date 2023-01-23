using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    PhotonView view;
    GameObject player;
    PlayerController playerController;
    [SerializeField] PlayerLoadout playerLoadout;
    [SerializeField] GameObject playerLoadoutUI;
    [SerializeField] GameObject playerLoadoutCamera;
    [SerializeField] Transform cameraHolder;
    GameObject loadoutCam;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] Slider mouseSlider;
    bool isPaused = false;
    bool inGame = false;

    int kills;
    int deaths;

    private void Awake()
    {
        PhotonNetwork.OfflineMode = true;
        view = GetComponent<PhotonView>();
        SetPlayerLoadout();
        if (view.IsMine)
        {
            mouseSlider.value = PlayerPrefs.GetFloat("Mouse Sensitivity", 300f);
        }
    }

    private void Update()
    {
        if (!view.IsMine)
            return;
        Pause();
    }

    public void SliderMouseSensitivity()
    {
        playerController.mouseSens = mouseSlider.value / 100;
        PlayerPrefs.SetFloat("Mouse Sensitivity", mouseSlider.value);
    }

    void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            if (inGame)
            {
                playerController.canMove = !isPaused;
                if (isPaused)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            pauseMenu.SetActive(isPaused);
        }
    }

    public PlayerLoadout GetPlayerLoadout()
    {
        return playerLoadout;
    }


    public void SetPlayerLoadout()
    {
        if (view.IsMine)
        {
            playerLoadoutUI.SetActive(true);
            loadoutCam = Instantiate(playerLoadoutCamera, cameraHolder.position, cameraHolder.rotation, cameraHolder);
        }
        else
        {
            Destroy(playerLoadoutUI);
        }
    }

    public void CreatePlayer()
    {
        if (!view.IsMine)
            return;
        Destroy(loadoutCam);
        //When Loadout set, create the player controller
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        //Instantiate the player controller
        player = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Player Controller"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { view.ViewID});
        playerController = player.GetComponent<PlayerController>();
        Debug.Log("Instantiated Player Controller");      
        playerLoadoutUI.SetActive(false);
        inGame = true;
    }


    public void Die()
    {
        if (view.IsMine)
            inGame = false;
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
