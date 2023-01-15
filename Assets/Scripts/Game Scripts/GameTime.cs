using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
public class GameTime : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text roundText;
    [SerializeField] TMP_Text gameTimeText;
    [SerializeField] int numRounds;
    [SerializeField] float roundTime;

    PhotonView view;

    int round = 1;
    float timeLeft;
    bool isGameDone = false;

    private void Awake()
    {
        timeLeft = roundTime;
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
    public void RPC_UpdateRound()
    {
        round += 1;
        if (round > numRounds)
            EndGame();
        else
        {
            timeLeft = roundTime;
            roundText.text = "Round: " + round.ToString();
            PlayerLoadout[] playerLoadouts = GameObject.FindObjectsOfType<PlayerLoadout>();
            foreach (PlayerLoadout playerLoadout in playerLoadouts)
                playerLoadout.RoundSkillIncrease();
        }
    }

    public void UpdateTime()
    {
        int roundIntA = Mathf.RoundToInt(timeLeft);
        timeLeft -= Time.deltaTime;
        int roundIntB = Mathf.RoundToInt(timeLeft);
        if (timeLeft <= 0f)
            view.RPC(nameof(RPC_UpdateRound), RpcTarget.All);
        if (roundIntA > roundIntB)
        {
            view.RPC(nameof(RPC_UpdateTimeText), RpcTarget.All, Mathf.CeilToInt(timeLeft));
        }

    }

    [PunRPC]
    public void RPC_UpdateTimeText(int time)
    {
        string minutes = ((int)(time / 60)).ToString();
        string seconds = (time % 60).ToString();
        if (seconds.Length == 1)
            seconds = '0' + seconds;
        gameTimeText.text = minutes + ":" + seconds;
    }

    public void EndGame()
    {
        isGameDone = true;
        Debug.Log("end game");
    }
}
