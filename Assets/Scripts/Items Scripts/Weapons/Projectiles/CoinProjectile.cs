using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CoinProjectile : Projectile
{
    [HideInInspector] public float coinRangeRadius;
    [HideInInspector] public float deflectDamage;
    [SerializeField] GameObject deflectPrefab;

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
        //targets the enemies head in range
        var cols = Physics.OverlapSphere(transform.position, coinRangeRadius);
        foreach (Collider col in cols)
        {
            IDamageable colDamage = col.gameObject.GetComponent<IDamageable>();
            PlayerController playerController = col.gameObject.GetComponentInParent<PlayerController>();
            if (col.CompareTag("Head") && playerController != this.playerController && !projectileHit.Contains(colDamage.DamageTransform()))
            {
                colDamage.TakeDamage(deflectDamage);
                DeflectLine(col.transform);
                projectileHit.Add(colDamage.DamageTransform());
            }
        }
    }
    public void DeflectLine(Transform target)
    {
        Vector3 halfPoint = (transform.position +target.position)/2;
        transform.LookAt(target);
        GameObject deflectLine = Instantiate(deflectPrefab, halfPoint, transform.rotation);
        deflectLine.transform.localScale = new Vector3(0.1f, 0.1f, (transform.position-target.position).magnitude);
        Destroy(deflectLine, .75f);
    }
}
