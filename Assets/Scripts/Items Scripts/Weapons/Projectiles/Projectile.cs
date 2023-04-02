using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class Projectile : MonoBehaviourPunCallbacks, IDamageable                                                                                                                
{
    public Rigidbody rb;
    public Collider col;
    public GameObject explosionPrefab;
    public PhotonView view;

    //components set by projectile gun
    [HideInInspector] public float speed;
    [HideInInspector] public float health;
    [HideInInspector] public float damage;
    [HideInInspector] public float time;
    [HideInInspector] public bool explodes;
    [HideInInspector] public float exploDamage;
    [HideInInspector] public float explosionRadius;
    [HideInInspector] public float selfDamage;
    [HideInInspector] public float blastStrength;
    [HideInInspector] public float flockTime;
    [HideInInspector] public float earlyExplosionMultiplyer;
    [HideInInspector] public PlayerController playerController = null;

    List<Transform> projectileHit = new List<Transform>();  //lsit of transforms projectile hits
    List<Transform> explosionHit = new List<Transform>();  //list of transforms GameObjects explosion hits
    GameObject explosion;
    public virtual void SetProjectile(float speed, float health, float damage, float time, bool explodes, float explosionDamage = 0, float explosionRadius = 0, float selfDamage = 0, float blastStrength = 0, float flockTime = 0, float earlyExplosionMultiplyer = 0, PlayerController p = null)
    {
        //sets all projectile components, accessed by projecctile gun
        //some setters are only handled by this projectile, others must be
        //sent through RPC for synced information
        this.speed = speed;
        this.health = health;
        this.damage = damage;
        this.time = time;
        this.explodes = explodes;
        this.exploDamage = explosionDamage;
        this.explosionRadius = explosionRadius;
        this.selfDamage = selfDamage;
        this.blastStrength = blastStrength;
        this.flockTime = flockTime;
        this.earlyExplosionMultiplyer = earlyExplosionMultiplyer;
        this.playerController = p;
        view.RPC(nameof(RPC_SetProjectile), RpcTarget.Others, explodes, explosionRadius);
    }
    [PunRPC]
    public void RPC_SetProjectile(bool ex, float radius)
    {
        explodes = ex;
        explosionRadius = radius;
    }


    public virtual void Start()
    {
        if (view.IsMine)
        {
            rb.velocity = transform.forward * speed;
            DestroyProjectile(time);

        }
        else
        {
            Destroy(rb);
            Destroy(col);
        }
    }
    public virtual void OnTriggerEnter(Collider col)
    {
        if (view.IsMine)
        {
            IDamageable colDamage = col.gameObject.GetComponent<IDamageable>();
            if (colDamage != null && !projectileHit.Contains(colDamage.DamageTransform()))
            {
                colDamage.TakeDamage(damage);
                projectileHit.Add(colDamage.DamageTransform());
            }
            if (explodes)
            {
                this.col.enabled = false;
                Explosion(exploDamage);
            }
            DestroyProjectile(0);
        }
    }

    public void AddSpeed(float addSpeed)
    {
        if(view.IsMine)
            speed += addSpeed;
    }

    public Transform DamageTransform()
    {
        return gameObject.transform;
    }
    public virtual void TakeDamage(float damage)
    {
        view.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage);
    }

    [PunRPC]
    public virtual void RPC_TakeDamage(float damage)
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
        var cols = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider col in cols)
        {
            IDamageable colDamage = col.gameObject.GetComponent<IDamageable>();
            if (!col.CompareTag("Head") && colDamage != null && !explosionHit.Contains(colDamage.DamageTransform()))
            {
                //if player, take the normalized vector between projectile impact and player to blast them away and add flock time
                PlayerController playerController = col.gameObject.GetComponentInParent<PlayerController>();
                if (playerController != null)
                {
                    if(playerController == this.playerController)
                    {
                        playerController.AddPlayerForce(
                            (playerController.playerTransform.position - transform.position).normalized * blastStrength);
                        playerController.flockTime += flockTime;
                        colDamage.TakeDamage(selfDamage);
                    }
                    else
                    {
                        colDamage.TakeDamage(exploDamage);
                    }
                }
                else
                {
                    colDamage.TakeDamage(exploDamage);
                }
                explosionHit.Add(colDamage.DamageTransform());
            }
        }
        view.RPC(nameof(RPC_Explosion), RpcTarget.All, damage, transform.position.x, transform.position.y, transform.position.z);
    }

    [PunRPC]
    public void RPC_Explosion(float damage, float x, float y, float z)
    {
        GameObject explosion = Instantiate(explosionPrefab, new Vector3(x,y,z), Quaternion.identity);
        explosion.transform.localScale = new Vector3(2 * explosionRadius, 2 * explosionRadius, 2 * explosionRadius);
        Destroy(explosion, .5f);
        Destroy(gameObject);
    }

    public void DestroyProjectile(float time)
    {
        view.RPC(nameof(RPC_DestoryProjectile), RpcTarget.All, time);
    }
    [PunRPC]
    public void RPC_DestoryProjectile(float time)
    {
        Destroy(gameObject, time);
    }

}
