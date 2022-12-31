using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView playerView;
    [SerializeField] TMP_Text text;

    private void Start()
    {
        if (playerView.IsMine)
        {
            gameObject.SetActive(false);
        }
        text.text = playerView.Owner.NickName;
    }
}
