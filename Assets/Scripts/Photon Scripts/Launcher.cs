using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
     public bool offline;

    public static Launcher Instance;
    private static Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListPrefab;
    [SerializeField] GameObject playerListPrefab;
    [SerializeField] GameObject startDeathMatchButton;
    [SerializeField] GameObject startPracticeRange;
    [Space]
    int gameIndex;

    void Awake()
    {
        Instance = this;
    }

    public void StartLauncher(bool isOffline)
    {
        offline = isOffline;
        print("start launcher");
        Debug.Log("Connecting To Master");
        if (offline)
        {
            PhotonNetwork.OfflineMode = true;
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Joined Master");
        PhotonNetwork.AutomaticallySyncScene = true;
        if (offline)
        {
            PhotonNetwork.CreateRoom("Offline");
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public void CreateRoom()
    {
        if (roomNameInputField.text.Length > 0)
        {
            print("creating room");
            PhotonNetwork.CreateRoom(roomNameInputField.text, new RoomOptions {IsVisible = true });
            MenuManager.Instance.OpenMenu("loading");
        }
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startDeathMatchButton.SetActive(PhotonNetwork.IsMasterClient);
        startPracticeRange.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startDeathMatchButton.SetActive(PhotonNetwork.IsMasterClient);
        startPracticeRange.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGameMode(int buildIndex)
    {
        //level num must be same num as build index of game
        PhotonNetwork.LoadLevel(buildIndex);
    }

    public void LeaveRoom()
    {
        if (offline)
        {
            MenuManager.Instance.DisconnectGame();
        }
        else
        {
            PhotonNetwork.LeaveRoom();
            MenuManager.Instance.OpenMenu("loading");
        }
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }

        foreach (KeyValuePair<string, RoomInfo> entry in cachedRoomList)
        {
            Instantiate(roomListPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(cachedRoomList[entry.Key]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
