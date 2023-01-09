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

    PhotonView view;


    public override void Awake()
    {
        base.Awake();
        view = GetComponent<PhotonView>();
        ammo = ((WeaponInfo)itemInfo).ammo;
        spread = ((WeaponInfo)itemInfo).hipSpread;
    }

    private void Update()
    {
        if (!view.IsMine)
        {
            return;
        }
        CheckUse();
        CheckReload();
        Aim();
    }
    public override void Use()
    {
        if (ammo == 0)
            Reload();
        if (canShoot && view.IsMine && ammo != 0 && !reloading)
        {
            Shoot();
            nextShot = Time.time + ((WeaponInfo)itemInfo).fireRate;
            canShoot = false;
            if (ammo != -1)
                ammo--;
            UpdateAmmo();
        }

    }

    void Shoot()
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
