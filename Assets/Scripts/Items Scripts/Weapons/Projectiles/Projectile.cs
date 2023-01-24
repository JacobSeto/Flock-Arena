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
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] protected PhotonView view;

    //components set by projectile gun
    public float speed { get; set; }
    public float health { get; set; }
    public float damage { get; set; }
    public float time { get; set; }
    public  bool explodes { get; set; }
    public float exploDamage { get; set; }
    public float explosionRadius { get; set; }
    public float selfDamage { get; set; }
    public float blastStrength { get; set; }
    public float earlyExplosionMultiplyer { get; set; }
    public PlayerController playerController { get; set; } = null;
    //prevent multiple collisions
    bool hit = false;
    bool hitExplosion = false;

    GameObject explosion;

    public void SetProjectile(float sp, float h, float d, float t, bool e, float exD, float exR, float sD, float bS, float exM, PlayerController p)
    {
        //sets all projectile components, accessed by projecctile gun
        speed = sp;
        health = h;
        damage = d;
        time = t;
        explodes = e;
        exploDamage = exD;
        explosionRadius = exR;
        selfDamage = sD;
        blastStrength = bS;
        earlyExplosionMultiplyer = exM;
        playerController = p;
    }
    public virtual void Start()
    {
        rb.velocity = transform.forward * speed;
        ProjectileDestroy(time);
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        if (view.IsMine && !hit)
        {
            if(other.gameObject.GetComponentInParent<IDamageable>() != null)
            {
                print("take damage");
                print(other.gameObject.name);
                other.gameObject.GetComponentInParent<IDamageable>().TakeDamage(damage);
            }
            if (explodes)
            {
                Explosion(exploDamage);
            }   
            
            ProjectileDestroy(0f);
            hit = true;
        }
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
                Explosion(exploDamage * earlyExplosionMultiplyer);
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
                    cols[i].GetComponentInParent<PlayerController>().AddPlayerForce(-transform.forward * blastStrength);
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
