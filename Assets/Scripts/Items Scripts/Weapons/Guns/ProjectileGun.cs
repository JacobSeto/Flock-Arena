using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProjectileGun : Weapon
{
    [SerializeField] PhotonView playerView;
    [SerializeField] Transform camHolder;
    [SerializeField] string projectileName;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] float projectileTime;


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
        GameObject projectile = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Projectiles", projectileName), projectileSpawn.position, camHolder.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.playerController = playerController;
        projectileScript.playerView = playerView;
        projectileScript.projectileDamage = ((WeaponInfo)itemInfo).damage;
        projectileScript.ProjectileDestroy(projectileTime);
    }

}
