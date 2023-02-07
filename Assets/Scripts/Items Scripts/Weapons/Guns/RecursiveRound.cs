using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecursiveRound : ProjectileGun
{
    [Space]
    public int numRecursive;
    public string recursiveRoundName;
    public float recursiveSpeed;
    public float recursiveHealth;
    public float recursiveDamage;
    public float recursiveTime;
    public float recursiveExploDamage;
    public float recursiveRadius;
    public float recursiveSelfDamage;
    public float recursiveBlastStrength;
    public float recursiveBlastAirTime;
    public float recursiveEEM;

    public void RecursiveUpgrades()
    {
        //Checks each recursive round upgrade and applies to gun
    }

    public override void Shoot()
    {
        base.Shoot();
        RecursiveProjectile recursiveScript = projectile.GetComponent<RecursiveProjectile>();
        recursiveScript.r = gameObject.GetComponent<RecursiveRound>();
        recursiveScript.numRecursive = numRecursive;
        recursiveScript.recursiveRoundName = recursiveRoundName;
    }

}
