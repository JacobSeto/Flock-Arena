using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GunGun : ProjectileGun
{
    [SerializeField] float boostTime;
    [SerializeField] float boostStrength;
    public GunGunProjectile gungunScript;

    public override void Awake()
    {
        base.Awake();
    }
    public void GunGunUpgrades()
    {
        //Checks each gungun upgrade and applies to gun
    }

    public override void Shoot()
    {
        base.Shoot();
        gungunScript = projectile.GetComponent<GunGunProjectile>();
        gungunScript.boostTime = boostTime;
        gungunScript.boostStrength = boostStrength;
    }
}
