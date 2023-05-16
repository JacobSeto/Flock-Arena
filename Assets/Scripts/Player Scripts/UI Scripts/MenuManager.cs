using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public static MenuManager Instance;
    [SerializeField] Menu[] menus;

    void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        for(int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void LeaveGame()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Destroy(RoomManager.Instance.gameObject);
            if (PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.Disconnect();
                SceneManager.LoadScene("Menu");
            }
            else
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LoadLevel(0);
            }
        }
    }

    public void DisconnectGame()
    {
        PhotonNetwork.Disconnect();
        OpenMenu("start");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
