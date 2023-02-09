using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CoinProjectile : Projectile
{
    [HideInInspector] public float coinRangeRadius;
    [HideInInspector] public float deflectDamage;

    [PunRPC]
    public override void RPC_TakeDamage(float damage)
    {
        if (view.IsMine)
            DeflectBullet();
        base.RPC_TakeDamage(damage);
    }

    public void DeflectBullet()
    {
        //When coin is shot, a revolver shot originating from the coin position
        //targets the enemies in range
        var cols = Physics.OverlapSphere(transform.position, coinRangeRadius);
        List<PlayerController> playersHit = new List<PlayerController>();
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].GetComponentInParent<PlayerController>() != null)
            {
                PlayerController player = cols[i].GetComponentInParent<PlayerController>();
                if (!playersHit.Contains(player) &&  player.view != playerController.view)
                {
                    playersHit.Add(player);
                    cols[i].GetComponentInParent<IDamageable>().TakeDamage(deflectDamage);
                }
            }
        }
    }
}
