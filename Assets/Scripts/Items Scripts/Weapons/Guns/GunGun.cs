using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GunGun : ProjectileGun
{
    [Header("Gun Gun Projectile")]
    public float boostTime;
    public float boostStrength;
    public float gungunDamage;
    public float gungunFirerate;
    public float gungunSpread;
    public float turnMax;
    public override void Awake()
    {
        base.Awake();
    }

    public override void Shoot()
    {
        base.Shoot();
        GunGunProjectile gungunScript = projectile.GetComponent<GunGunProjectile>();
        gungunScript.boostTime = boostTime;
        gungunScript.boostStrength = boostStrength;
        gungunScript.singleShotGun.damage = gungunDamage;
        gungunScript.singleShotGun.fireRate = gungunFirerate;
        gungunScript.singleShotGun.hipSpread = gungunSpread;
        gungunScript.singleShotGun.recoil = recoil;
        gungunScript.turnMax = turnMax;
        
    }


    public override void Special()
    {
        //Gun Gun Special:  Launch a homing projectile
    }
}
