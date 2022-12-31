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
        other.gameObject.GetComponent<IDamageable>()?.TakeDamage(GetComponentInParent<Weapon>().GetDamage());
        gameObject.SetActive(false);
    }
}
