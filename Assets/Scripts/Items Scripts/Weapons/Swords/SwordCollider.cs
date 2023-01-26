using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView view;
    private void OnTriggerEnter(Collider other)
    {
        if (!view.IsMine)
            return;
        other.gameObject.GetComponentInParent<IDamageable>()?.TakeDamage(GetComponentInParent<Weapon>().damage);
        gameObject.SetActive(false);
    }
}
