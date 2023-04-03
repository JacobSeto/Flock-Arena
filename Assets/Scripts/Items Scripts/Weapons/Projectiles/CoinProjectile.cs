using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CoinProjectile : Projectile
{
    [HideInInspector] public float coinRangeRadius;
    [HideInInspector] public float deflectMultiplyer;
    [SerializeField] GameObject deflectPrefab;

    public override void TakeDamage(float damage)
    {
        if (view.IsMine)
        {
            DeflectBullet(damage * deflectMultiplyer);
        }
        base.TakeDamage(damage);
    }

    public void DeflectBullet(float deflectDamage)
    {
        //When coin is shot, a revolver shot originating from the coin position
        //targets the enemies head in range.  Other guns can damage coin
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
        Vector3 targetPosition = target.position;
        Vector3 halfPoint = (transform.position + targetPosition) / 2;
        transform.LookAt(target);
        view.RPC(nameof(RPC_DelfectLine), RpcTarget.All, halfPoint.x,halfPoint.y, halfPoint.z,
            targetPosition.x,targetPosition.y,targetPosition.z);
    }
    //pass vectors as their x,y, and z components
    [PunRPC]
    public void RPC_DelfectLine(float hX, float hY, float hZ, float tX, float tY, float tZ)
    {
        Vector3 halfPoint = new Vector3(hX, hY, hZ);
        Vector3 targetPosition = new Vector3(tX, tY, tZ);
        GameObject deflectLine = Instantiate(deflectPrefab, halfPoint, transform.rotation);
        deflectLine.transform.localScale = new Vector3(0.1f, 0.1f, (transform.position - targetPosition).magnitude);
        Destroy(deflectLine, .75f);
    }
}
