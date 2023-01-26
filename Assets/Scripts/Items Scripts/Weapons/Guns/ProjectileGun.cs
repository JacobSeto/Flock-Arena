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
    public float projectileSpeed;
    public float projectileHealth;
    public float projectileDamage;
    public float projectileTime;
    [Space]
    public bool explodes;
    public float exploDamage;
    public float explosionRadius;
    public float selfDamage;
    public float blastStrength;
    public float earlyExplosionMultiplyer;

    public GameObject projectile;


    public override void Awake()
    {
        base.Awake();
        ammo = ((WeaponInfo)itemInfo).ammo;
        spread = ((WeaponInfo)itemInfo).hipSpread;
    }

    public override void Shoot()
    {
        
        if (!view.IsMine)
            return;
        projectile = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Projectiles", projectileName), projectileSpawn.position, playerController.camTransform.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.SetProjectile(projectileSpeed, projectileHealth, projectileDamage, projectileTime,
        explodes, exploDamage, explosionRadius, selfDamage, blastStrength, earlyExplosionMultiplyer,playerController);

    }



}
