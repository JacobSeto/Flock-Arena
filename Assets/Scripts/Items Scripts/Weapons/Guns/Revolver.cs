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
    public float deflectMultiplyer; //damaged multiplied when shot
    [HideInInspector] public GameObject coin;

    //Revolver
    [Header("Revolver Path 1")]
    public float heavyChange;  //increase damage of coin hitting players directly
    public float moneyHandsCooldown;  //shorter coin cooldown
    public float silverDollarMultiplyer;  //coin deflect multipler increased
    public int titaniumHealth;  //increase health of coin to be shot multiple times

    [Header("Revolver Path 2")]
    public float betterBarrelReloadTime; //faster reload time
    public float quickDrawTime;  // aim-in much faster
    [HideInInspector] public bool explosionRounds; //Explosive Round:  revolver shots now explod on impact
    [SerializeField] string exploRoundName;
    [SerializeField] float exploDamage;
    [SerializeField] float exploRadius;
    GameObject explosiveRound;
    public float revautoFirerate; //revauto:  auto fire enabled and faster firerate


    [Header("Revolver Path 3")]
    public float largerMagazines;  //increase the ammo capacity of both revolver and rifle.  multiply ammo
    public int numFrag;  //when shotgun shoots coin, the coin fragments and shoots multiple projectiles
    public float oiledBulletsDamageBonus;  //RPG projectiles shot by revolver do extra explosion damage
    public float recursiveCoinRadiusBonus; //When coin hits recursive round (not projectile), increase explosion radius



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
        explosiveRound = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Projectiles", exploRoundName), hitPoint, Quaternion.identity);
        Projectile projectileScript = explosiveRound.GetComponent<Projectile>();
        projectileScript.SetProjectile(0, 1, 0, 1,
        true, exploDamage, exploRadius, 25, 200, 1, 1, playerController);
    }


}
