using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecursiveRound : ProjectileGun
{
    [SerializeField] int numRecursive;
    [SerializeField] string recursiveRound;
    public void RecursiveUpgrades()
    {
        //Checks each recursive round upgrade and applies to gun
    }

    public override void Shoot()
    {
        if (!view.IsMine)
            return;
        GameObject projectile = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Projectiles", projectileName), projectileSpawn.position, playerController.camTransform.rotation);
        RecursiveProjectile projectileScript = projectile.GetComponent<RecursiveProjectile>();
        projectileScript.numRecursive = numRecursive;
        projectileScript.recursiveRound = recursiveRound;
        projectileScript.playerController = playerController;
    }

}
