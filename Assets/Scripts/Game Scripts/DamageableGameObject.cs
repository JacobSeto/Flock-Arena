using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class DamageableGameObject : MonoBehaviourPunCallbacks, IDamageable
{
    public PhotonView view;
    public bool invincible;
    public bool displayDamage;
    public float health;
    public GameObject damageDisplayPrefab;
    public Transform damageDisplay;

    public Transform DamageTransform()
    {
        return gameObject.transform;
    }

    public void TakeDamage(float damage)
    {
        view.RPC(nameof(RPC_TakeDamage), view.Owner, damage);
        if(displayDamage)
            view.RPC(nameof(RPC_DisplayDamage), RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        if (invincible || damage == 0)
        {
            return;
        }
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    [PunRPC]
    void RPC_DisplayDamage(float damage)
    {
        //display damage on other players' screens
        GameObject damageGameObject = Instantiate(damageDisplayPrefab, damageDisplay.position, damageDisplay.rotation, damageDisplay);
        damageGameObject.GetComponentInChildren<TMP_Text>().text = damage.ToString();
        Destroy(damageGameObject, 1);
    }

}
