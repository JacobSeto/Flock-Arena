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
    [HideInInspector] public float blastAirTime;
    [HideInInspector] public float earlyExplosionMultiplyer;
    [HideInInspector] public PlayerController playerController = null;
    //prevent multiple collisions
    public bool hit = false;
    bool hitExplosion = false;

    GameObject explosion;
    public virtual void SetProjectile(float sp, float h, float d, float t, bool e, float exD = 0, float exR = 0, float sD = 0, float bS = 0, float bA = 0, float exM = 0, PlayerController p = null)
    {
        //sets all projectile components, accessed by projecctile gun
        //some setters are only handled by this projectile, others must be
        //sent through RPC for synced information
        speed = sp;
        health = h;
        damage = d;
        time = t;
        explodes = e;
        exploDamage = exD;
        explosionRadius = exR;
        selfDamage = sD;
        blastStrength = bS;
        blastAirTime = bA;
        earlyExplosionMultiplyer = exM;
        playerController = p;
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
    public virtual void OnTriggerEnter(Collider other)
    {
        if (view.IsMine && !hit)
        {
            if(other.gameObject.GetComponentInParent<IDamageable>() != null)
            {
                other.gameObject.GetComponentInParent<IDamageable>().TakeDamage(damage);
            }
            if (explodes)
            {
                Explosion(exploDamage);
            }   
            
            DestroyProjectile(0);
            hit = true;
        }
    }

    public void AddSpeed(float addSpeed)
    {
        if(view.IsMine)
            speed += addSpeed;
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
        view.RPC(nameof(RPC_Explosion), RpcTarget.All, damage, transform.position.x, transform.position.y, transform.position.z);
    }

    [PunRPC]
    public void RPC_Explosion(float damage, float x, float y, float z)
    {
        GameObject explosion = Instantiate(explosionPrefab, new Vector3(x,y,z), Quaternion.identity);
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
                    cols[i].GetComponentInParent<PlayerController>().airTime += blastAirTime;
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
