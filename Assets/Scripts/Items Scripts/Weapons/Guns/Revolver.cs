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
    public float coinRangeRadius;  //The range of the coin can deflect the bullet
    public float deflectDamage;
    [HideInInspector] public GameObject coin;



    public override void Special()
    {
        //Revolver Special: Shoot a coin projectile.  When shot,
        //shoot at enemies in range
        coin = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Projectiles", coinName), coinSpawn.position, coinSpawn.rotation);
        CoinProjectile coinScript = coin.GetComponent<CoinProjectile>();
        coinScript.SetProjectile(coinSpeed, coinHealth, coinDamage, coinTime,
        false, 0, 0, 0,0, 0, 0, playerController);
        coinScript.coinRangeRadius = coinRangeRadius;
        coinScript.deflectDamage = deflectDamage;
    }


}
