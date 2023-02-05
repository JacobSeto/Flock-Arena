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
    public float speed;
    public float health;
    public float damage;
    public float time;
    public bool explodes;
    public float exploDamage;
    public float explosionRadius;
    public float selfDamage;
    public float blastStrength;
    public float blastAirTime;
    public float earlyExplosionMultiplyer;
    public PlayerController playerController = null;
    //prevent multiple collisions
    bool hit = false;
    bool hitExplosion = false;

    GameObject explosion;

    public void SetProjectile(float sp, float h, float d, float t, bool e, float exD, float exR, float sD, float bS, float bA, float exM, PlayerController p)
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
        blastAirTime = bA;
        earlyExplosionMultiplyer = exM;
        playerController = p;
    }
    public virtual void Start()
    {
        rb.velocity = transform.forward * speed;
        print(time);
        Destroy(gameObject, time);
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
            
            Destroy(gameObject);
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

}
