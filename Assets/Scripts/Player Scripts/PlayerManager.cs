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
    public PhotonView view;
    public GameObject player;
    public PlayerController playerController;
    public PlayerLoadout playerLoadout;
    public GameObject playerLoadoutUI;
    [SerializeField] GameObject playerLoadoutCamera;
    [SerializeField] Transform cameraHolder;
    GameObject loadoutCam;


    [SerializeField] GameObject pauseMenu;
    [SerializeField] Slider mouseSlider;
    public bool inGame = false;
    bool isPaused = false;
    public int kills;
    public int deaths;

    [HideInInspector] public bool isPlayer;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        SetPlayerLoadout();
    }

    private void Update()
    {
        if (!view.IsMine)
            return;
        Pause();
    }

    public void SliderMouseSensitivity()
    {
        if (!view.IsMine)
            return;
        playerController.mouseSens = mouseSlider.value / 100;
        PlayerPrefs.SetFloat("Mouse Sensitivity", mouseSlider.value);
    }

    void Pause()
    {
        if (!view.IsMine)
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            if (inGame)
            {
                playerController.isPaused = !playerController.isPaused;
                if (playerController.isPaused)
                {
                    Cursor.lockState = CursorLockMode.None;
                    playerController.canMove = false;
                    playerController.playerUI.SetActive(false);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    playerController.canMove = true;
                    playerController.playerUI.SetActive(true);
                }
            }
            pauseMenu.SetActive(isPaused);
        }
    }


    public void SetPlayerLoadout()
    {
        if (view.IsMine)
        {
            mouseSlider.value = PlayerPrefs.GetFloat("Mouse Sensitivity", 300f);
            isPlayer = true;
            playerLoadoutUI.SetActive(true);
            loadoutCam = Instantiate(playerLoadoutCamera, cameraHolder.position, cameraHolder.rotation, cameraHolder);
        }
        else
        {
            Destroy(playerLoadoutUI);
            isPlayer = false;
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
        print("before");
        player = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Player Controller"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { view.ViewID});
        print("after");
        playerController = player.GetComponent<PlayerController>();   
        playerLoadoutUI.SetActive(false);
        inGame = true;
    }


    public void Die()
    {
        if (!view.IsMine)
        {
            return;
        }
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
