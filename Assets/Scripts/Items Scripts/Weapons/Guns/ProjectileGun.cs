using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProjectileGun : Weapon
{
    public string projectileName;
    public Transform projectileSpawn;
    [Space]
    public float hipProjectileSpeed;
    public float aimProjectileSpeed;
    float projectileSpeed;
    public float projectileHealth;
    public float projectileDamage;
    public float projectileTime;
    [Space]
    public bool explodes;
    public float exploDamage;
    public float explosionRadius;
    public float selfDamage;
    public float blastStrength;
    public float blastAirTime;
    public float earlyExplosionMultiplyer;

    [HideInInspector] public GameObject projectile;


    public override void Awake()
    {
        base.Awake();
        ammo = ((WeaponInfo)itemInfo).ammo;
        spread = ((WeaponInfo)itemInfo).hipSpread;
    }

    public override void Shoot()
    {
        base.Shoot();
        if (!view.IsMine)
            return;
        projectile = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Projectiles", projectileName), projectileSpawn.position, projectileSpawn.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.SetProjectile(projectileSpeed, projectileHealth, projectileDamage, projectileTime,
        explodes, exploDamage, explosionRadius, selfDamage, blastStrength, blastAirTime,earlyExplosionMultiplyer,playerController);

    }

    public override void Aim()
    {
        base.Aim();
        //The longer you aim, the faster your next projectile will become
        if (Input.GetKey(KeyCode.Mouse1))
        {
            IncreaseProjectileSpeed();
        }
        else
        {
            DecreaseProjectileSpeed();
        }
    }
    void IncreaseProjectileSpeed()
    {
        projectileSpeed = Mathf.Lerp(projectileSpeed, aimProjectileSpeed, ((WeaponInfo)itemInfo).aimSpeed * Time.deltaTime);
    }

    void DecreaseProjectileSpeed()
    {
        projectileSpeed = Mathf.Lerp(projectileSpeed, hipProjectileSpeed, ((WeaponInfo)itemInfo).hipSpeed * Time.deltaTime);
    }



}
