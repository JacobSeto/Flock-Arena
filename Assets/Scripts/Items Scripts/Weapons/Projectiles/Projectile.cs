using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class Projectile : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] Collider col;
    [SerializeField] float projectileDamage;
    [SerializeField] float explosionDamage;
    [SerializeField] protected float speed;
    [SerializeField] float health;
    [SerializeField] protected float projectileTime;
    [Space]
    [SerializeField] bool explodes;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float explosionRadius;
    [SerializeField] float selfDamage = 10f;
    [SerializeField] float explosionBlastStrength;
    [SerializeField] float earlyExplosionMultiplyer;
    [SerializeField] protected PhotonView view;

    //prevent multiple collisions
    bool hit = false;
    bool hitExplosion = false;
    public PlayerController playerController { get; set; } = null;

    GameObject explosion;

    public virtual void Start()
    {
        rb.velocity = transform.forward * speed;
        ProjectileDestroy(projectileTime);
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        if (view.IsMine && !hit)
        {
            if(other.gameObject.GetComponentInParent<IDamageable>() != null)
            {
                print("take damage");
                print(other.gameObject.name);
                other.gameObject.GetComponentInParent<IDamageable>().TakeDamage(projectileDamage);
            }
            if (explodes)
            {
                Explosion(explosionDamage);
            }   
            
            ProjectileDestroy(0f);
            hit = true;
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
            if(explodes)
                Explosion(explosionDamage * earlyExplosionMultiplyer);
            else
            {
                Destroy(gameObject);
            }
        }
    }
    public void Explosion(float damage)
    {
        view.RPC(nameof(PUNExplosion), RpcTarget.All, damage);
    }

    [PunRPC]
    public void PUNExplosion(float damage)
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.transform.localScale = new Vector3(2 * explosionRadius, 2 * explosionRadius, 2 * explosionRadius);
        Destroy(explosion, .5f);
        if (!view.IsMine || hitExplosion)
            return;
        var cols = Physics.OverlapSphere(transform.position, explosionRadius);

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].GetComponentInParent<PlayerController>() != null)
            {
                PlayerController player = cols[i].GetComponentInParent<PlayerController>();
                if (player.view == playerController.view)
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
                hitExplosion = true;
            }
            
        }
        Destroy(gameObject);
    }

    public virtual void ProjectileDestroy(float delay)
    {
        view.RPC(nameof(PUNProjectileDestroy), RpcTarget.All, delay);
    }

    [PunRPC]
    public void PUNProjectileDestroy(float delay)
    {
        Destroy(gameObject, delay);
    }

}
