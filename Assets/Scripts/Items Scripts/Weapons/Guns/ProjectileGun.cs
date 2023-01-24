using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProjectileGun : Weapon
{
    [SerializeField] string projectileName;
    [SerializeField] protected Transform projectileSpawn;
    [Space]
    [SerializeField] float projectileSpeed;
    [SerializeField] float projectileHealth;
    [SerializeField] float projectileDamage;
    [SerializeField] float projectileTime;
    [Space]
    [SerializeField] bool explodes;
    [SerializeField] float exploDamage;
    [SerializeField] float explosionRadius;
    [SerializeField] float selfDamage;
    [SerializeField] float blastStrength;
    [SerializeField] float earlyExplosionMultiplyer;


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
        GameObject projectile = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Projectiles", projectileName), projectileSpawn.position, playerController.camTransform.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.SetProjectile(projectileSpeed, projectileHealth, projectileDamage, projectileTime,
        explodes, exploDamage, explosionRadius, selfDamage, blastStrength, earlyExplosionMultiplyer,playerController);

    }



}
