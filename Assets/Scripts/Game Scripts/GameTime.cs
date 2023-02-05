using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
public class GameTime : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject gameHUD;
    [SerializeField] TMP_Text roundText;
    [SerializeField] TMP_Text gameTimeText;
    [SerializeField] int numRounds;
    [SerializeField] float roundTime;
    [SerializeField] float startTime;

    PhotonView view;

    int round = 0;  //round 0 is prepare round before game starts
    float timeLeft;
    bool isGameDone = false;

    private void Awake()
    {
        timeLeft = startTime;
        view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (view.IsMine && !isGameDone)
        {
            UpdateTime();
        }
    }

    [PunRPC]
    public void RPC_UpdateRound(int _round)
    {
        round = _round;
        if (round > numRounds)
            EndGame();
        else
        {

            timeLeft = roundTime;
            roundText.text = "Round: " + round.ToString();
            //add skill points as long as it isn't starting round
            PlayerManager[] playerManagers = GameObject.FindObjectsOfType<PlayerManager>();
            if (round == 1)
            {
                foreach (PlayerManager playerManager in playerManagers)
                {
                    if(playerManager.isPlayer)
                        playerManager.playerLoadout.SpawnButton.SetActive(true);
                }
            }
            else
            {
                foreach (PlayerManager playerManager in playerManagers)
                {
                    if (playerManager.isPlayer)
                    {
                        playerManager.playerLoadout.RoundSkillIncrease();
                        playerManager.playerLoadout.SpawnButton.SetActive(true);

                    }

                }

            }
            gameHUD.SetActive(true);
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
            UpdateStartCounter(time);
        string minutes = ((int)(time / 60)).ToString();
        string seconds = (time % 60).ToString();
        if (seconds.Length == 1)
            seconds = '0' + seconds;
        gameTimeText.text = minutes + ":" + seconds;
    }

    public void UpdateStartCounter(int time)
    {
        //Updates the start counter in the player loadout before player can spawn in
        PlayerManager[] playerManagers = GameObject.FindObjectsOfType<PlayerManager>();
        foreach (PlayerManager playerManager in playerManagers)
        {
            //set empty if reached 0
            if (time == 0)
                playerManager.playerLoadout.StartCountdown.text = "";
            else
                playerManager.playerLoadout.StartCountdown.text = time.ToString();
        }
    }

    public void EndGame()
    {
        isGameDone = true;
        Debug.Log("end game");
    }
}
