using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Rifle : SingleShotGun
{
    [Header("Rifle Boomerang")]
    public string boomerName;
    public Transform boomerSpawn;
    public float boomerSpeed;
    public float boomerHealth;
    public float boomerDamage;
    public float boomerTime;
    [HideInInspector] public GameObject boomerang;

    public void RevolverUpgrades()
    {
        //Checks each rifle upgrade and applies to gun
        
    }

    public override void Special()
    {
        //Rifle Special: Throw rifle like a boomerang, shoots while
        // spinning.  When hit object, return
        boomerang = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Projectiles", boomerName), boomerSpawn.position, boomerSpawn.rotation);
        RifleProjectile boomerScript = boomerang.GetComponent<RifleProjectile>();
        boomerScript.SetProjectile(boomerSpeed, boomerHealth, boomerDamage, boomerTime,
        false, 0, 0, 0, 0, 0, 0, playerController);
        boomerScript.rifle = this;
        RifleActive(false);
        
    }

    public void RifleActive(bool isActive)
    {
        //sets the gameobject Active state
        view.RPC(nameof(RPC_RifleActive), RpcTarget.All, isActive);
    }

    [PunRPC]
    public void RPC_RifleActive(bool isActive)
    {
        canShoot = isActive;
       itemGameObject.SetActive(isActive);
    }

}
