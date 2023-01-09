using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class Projectile : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider col;
    [SerializeField] float speed;
    [SerializeField] float health;
    [Space]
    [SerializeField] bool explodes;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float explosionRadius;
    [SerializeField] float explosionDamage;
    [SerializeField] float selfDamage = 10f;
    [SerializeField] float explosionBlastStrength;
    [SerializeField] float earlyExplosionMultiplyer;
    bool hit = false;
    bool exploaded = false;
    [SerializeField] PhotonView view;
    public PhotonView playerView {get; set; }
    public float projectileDamage {get; set;}
    public PlayerController playerController { get; set;}

    GameObject explosion;

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (health > 0 && !hit && view.IsMine)
        {
            print(projectileDamage);
            if(other.gameObject.GetComponentInParent<IDamageable>() != null)
            {
                other.gameObject.GetComponentInParent<IDamageable>().TakeDamage(projectileDamage);
                hit = true;
            }
            if (explodes && !exploaded)
            {
                Explosion(explosionDamage);
            }
            if (hit)
            {
                ProjectileDestroy(0f);
            }
        }
    }

    public float GetExplosionDamage()
    {
        return explosionDamage;
    }

    public void AddSpeed(float addSpeed)
    {
        speed += addSpeed;
    }
    public void TakeDamage(float damage)
    {
        view.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage);
    }

    [PunRPC]
    public void RPC_TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            //exploding it early does 4 times damage
            if(explodes && !exploaded)
                Explosion(explosionDamage * earlyExplosionMultiplyer);
            else
            {
                Destroy(gameObject);
            }
        }
    }
    public void Explosion(float damage)
    {
        exploaded = true;
        view.RPC(nameof(PUNExplosion), RpcTarget.All, damage);
    }

    [PunRPC]
    public void PUNExplosion(float damage)
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.transform.localScale = new Vector3(2 * explosionRadius, 2 * explosionRadius, 2 * explosionRadius);
        Destroy(gameObject);
        Destroy(explosion, .5f);
        var cols = Physics.OverlapSphere(transform.position, explosionRadius);

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].GetComponentInParent<PlayerController>() != null)
            {
                if (cols[i].GetComponentInParent<PlayerController>().view == playerController.view)
                {
                    cols[i].GetComponentInParent<PlayerController>().AddPlayerForce(-transform.forward * explosionBlastStrength);
                    cols[i].GetComponentInParent<IDamageable>().TakeDamage(selfDamage);
                    i += cols.Length;
                }
                else
                {
                    cols[i].GetComponentInParent<IDamageable>().TakeDamage(damage);
                    i += cols.Length;
                }
            }
            
        }
    }

    public void ProjectileDestroy(float delay)
    {
        view.RPC(nameof(PUNProjectileDestroy), RpcTarget.All, delay);
    }

    [PunRPC]
    public void PUNProjectileDestroy(float delay)
    {
        Destroy(gameObject, delay);
    }

}
