using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class Revolver : SingleShotGun
{
    [Header("Coin")]
    public string coinName;
    public Transform coinSpawn;
    public float coinSpeed;
    public float coinHealth;
    public float coinDamage;
    public float coinTime;
    [HideInInspector] public GameObject coin;

    [Header("Upgrades")]
    [SerializeField] float quickHandReloadTime;
    [HideInInspector] public bool quickHands = false;

    public void RevolverUpgrades()
    {
        //Checks each revolver upgrade and applies to gun
        if (quickHands)
        {
            reload = quickHandReloadTime;
        }
    }

    public override void Special()
    {
        //Revolver Special: Shoot a coin projectile.  When shot,
        //shoot at nearest enemy
        coin = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Projectiles", coinName), coinSpawn.position, playerController.camTransform.rotation);
        CoinProjectile coinScript = coin.GetComponent<CoinProjectile>();
        coinScript.SetProjectile(coinSpeed, coinHealth, coinDamage, coinTime,
        false, 0, 0, 0,0, 0, 0, playerController);
    }


}
