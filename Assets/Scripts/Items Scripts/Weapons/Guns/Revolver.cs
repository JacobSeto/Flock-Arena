using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class Revolver : SingleShotGun
{
    public bool explosionRounds;
    [SerializeField] string exploName;
    [SerializeField] float exploDamage;
    [SerializeField] float exploRadius;
    GameObject explosiveRound;

    [Header("Coin")]
    public string coinName;
    public Transform coinSpawn;
    public float coinSpeed;
    public float coinHealth;
    public float coinDamage;
    public float coinTime;
    public float coinRangeRadius;  //The range of the coin can deflect the bullet
    public float deflectMultiplyer; //damaged multiplied when shot
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
        coinScript.deflectMultiplyer = this.deflectMultiplyer;
    }

    public override void BulletHit(RaycastHit hit)
    {
        base.BulletHit(hit);
        if (explosionRounds)
            ExplosionRound(hit.point);
    }

    void ExplosionRound(Vector3 hitPoint)
    {
        explosiveRound = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Projectiles", exploName), hitPoint, Quaternion.identity);
        Projectile projectileScript = explosiveRound.GetComponent<Projectile>();
        projectileScript.SetProjectile(0, 1, 0, 1,
        true, exploDamage, exploRadius, 25, 200, 1, 1, playerController);
    }


}
