using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
public class GameTime : MonoBehaviourPunCallbacks
{
    public static GameTime Instance;
    [SerializeField] TMP_Text roundText;
    [SerializeField] TMP_Text gameTimeText;
    [SerializeField] int numRounds;
    [SerializeField] float roundTime;

    PhotonView view;

    int round;
    float timeLeft;
    bool isGameDone = false;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        Instance = this;
        UpdateRound();
    }

    private void Update()
    {
        if (view.IsMine && !isGameDone)
        {
            UpdateTime();
        }
    }

    public void UpdateRound()
    {
        round += 1;
        if (round > numRounds)
            EndGame();
        timeLeft = roundTime;
        roundText.text = round.ToString();
    }

    public void UpdateTime()
    {
        int roundIntA = Mathf.RoundToInt(timeLeft);
        timeLeft -= Time.deltaTime;
        int roundIntB = Mathf.RoundToInt(timeLeft);
        if (timeLeft <= 0f)
            UpdateRound();
        if (roundIntA > roundIntB)
        {
            view.RPC(nameof(RPC_UpdateTimeText), RpcTarget.All, Mathf.Ceil(timeLeft).ToString());
            Debug.Log("changed time");
        }

    }

    [PunRPC]
    public void RPC_UpdateTimeText(string time)
    {
        print("update time");
        gameTimeText.text = time;
    }

    public void EndGame()
    {
        isGameDone = true;
        Debug.Log("end game");
    }
}
