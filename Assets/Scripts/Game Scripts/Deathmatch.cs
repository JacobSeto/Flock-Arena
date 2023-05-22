using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System.IO;

public class Deathmatch : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject gameHUD;
    [SerializeField] TMP_Text roundText;
    [SerializeField] TMP_Text gameTimeText;
    [SerializeField] int numRounds;
    [SerializeField] float roundTime;
    [SerializeField] float startTime;

    [SerializeField] PhotonView view;

    int round = 0;  //round 0 is prepare round before game starts
    float timeLeft;
    bool isGameDone = false;

    //End Screen
    [SerializeField] GameObject EndScreen;
    [SerializeField] TMP_Text winnerText;
    [SerializeField] TMP_Text killsText;

    PlayerManager[] playerManagers;
    PlayerManager playerManager;

    private void Awake()
    {
        timeLeft = startTime;
        PhotonNetwork.AutomaticallySyncScene = false;

    }
    private void Start()
    {
        playerManagers = GameObject.FindObjectsOfType<PlayerManager>();
        foreach (PlayerManager playerManager in playerManagers)
        {
            if (playerManager.view.IsMine)
                this.playerManager = playerManager;
        }
    }

    private void Update()
    {
        if (view.IsMine && !isGameDone)
        {
            UpdateTime();
            if (round > numRounds)
                EndGame();
        }
    }

    [PunRPC]
    public void RPC_UpdateRound(int _round)
    {
        round = _round;

        timeLeft = roundTime;
        roundText.text = "Round: " + round.ToString();
        if(round == 1)
        {
            //beginning round
            playerManager.playerLoadout.SpawnButton.SetActive(true);
        }
        else
        {
            playerManager.playerLoadout.RoundSkillIncrease();
            playerManager.playerLoadout.SpawnButton.SetActive(true);
        }
    }

    public void UpdateTime()
    {
        int timeA = Mathf.FloorToInt(timeLeft);
        timeLeft -= Time.deltaTime;
        int timeB = Mathf.FloorToInt(timeLeft);
        if (timeA > timeB)
        {
            view.RPC(nameof(RPC_UpdateTimeText), RpcTarget.All, Mathf.CeilToInt(timeLeft));
        }
        if (timeLeft <= 0f)
        {
            round += 1;
            view.RPC(nameof(RPC_UpdateRound), RpcTarget.All, round);
        }

    }

    [PunRPC]
    public void RPC_UpdateTimeText(int time)
    {
        if (round == 0)
        {
            if (time == 0)
            {
                playerManager.playerLoadout.StartCountdown.text = "";
                gameHUD.SetActive(true);
            }
            else
                playerManager.playerLoadout.StartCountdown.text = time.ToString();
        }
        else
        {
            string minutes = ((int)(time / 60)).ToString();
            string seconds = (time % 60).ToString();
            if (seconds.Length == 1)
                seconds = '0' + seconds;
            gameTimeText.text = minutes + ":" + seconds;

        }
    }

    public void EndGame()
    {
        isGameDone = true;
        Debug.Log("end game");
        //kill all players and disable playerManager
        gameHUD.SetActive(false);
        int highestKill = 0;
        string winnerName = "!?";
        foreach (PlayerManager playerManager in playerManagers)
        {
            if(playerManager.kills > highestKill)
            {
                highestKill = playerManager.kills;
                winnerName = playerManager.view.Owner.NickName;
            }
        }
        view.RPC(nameof(RPC_EndGame), RpcTarget.All, winnerName, highestKill);
    }

    [PunRPC]
    public void RPC_EndGame(string winnerName, int highestKill)
    {
        if (playerManager.player != null)
            playerManager.Die();
        winnerText.text = winnerName + " Wins!!!";
        killsText.text = highestKill + " Kills";
        EndScreen.SetActive(true);
        playerManager.playerLoadoutUI.SetActive(false);
    }
}
