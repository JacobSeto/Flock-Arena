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
    public int boomerBounce;  //num times rifle bounces before despawns
    public float bounceBonusDamage;  //damage increase everytime rifle bounces
    public int bounceAmmoBonus; //ammo added to rifle for every bounce
    [HideInInspector] public GameObject boomerang;

    public void RevolverUpgrades()
    {
        //Checks each rifle upgrade and applies to gun
        
    }

    public override void Special()
    {
        //Rifle Special: Throw rifle like a boomerang, add extra ammo for every bounce
        // spinning.  When hit object, return
        boomerang = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Projectiles", boomerName), boomerSpawn.position, boomerSpawn.rotation);
        RifleProjectile boomerScript = boomerang.GetComponent<RifleProjectile>();
        boomerScript.SetProjectile(boomerSpeed, boomerHealth, boomerDamage, boomerTime,
        false, 0, 0, 0, 0, 0, 0, playerController);
        boomerScript.rifle = this;
        boomerScript.numBounceRemaining = boomerBounce;
        boomerScript.bounceBonus = bounceBonusDamage;
        RifleActive(false);
        reloading = false;
        UpdateItemUI();
        
    }

    public void RifleActive(bool isActive)
    {
        //sets the gameobject Active state
        if (isActive)
        {
            Reload();
        }
        view.RPC(nameof(RPC_RifleActive), RpcTarget.All, isActive);
    }

    [PunRPC]
    public void RPC_RifleActive(bool isActive)
    {
        toggleInactive = !isActive;
        if (isEquip)
        {
            ItemSetActive(isActive);
        }
    }

}
