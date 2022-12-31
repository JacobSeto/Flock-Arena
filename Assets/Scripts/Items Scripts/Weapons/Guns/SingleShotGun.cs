using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Weapon
{
    [SerializeField] AudioClip gunShot;

    PhotonView view;
    public override void Awake()
    {
        base.Awake();
        view = GetComponent<PhotonView>();
        ammo = ((WeaponInfo)itemInfo).ammo;
        spread = ((WeaponInfo)itemInfo).hipSpread;
    }

    private void Update()
    {
        if (!view.IsMine)
        {
            return;
        }
        CheckUse();
        CheckReload();
        Aim();
    }

    public override void Use()
    {
        if (ammo == 0)
            Reload();
        if (canShoot && view.IsMine && ammo != 0 && !reloading)
        {
            Shoot();
            nextShot = Time.time + ((WeaponInfo)itemInfo).fireRate;
            canShoot = false;
            if (ammo != -1)
                ammo--;
            UpdateAmmo();
        }
        
    }

    void Shoot()
    {
        Ray ray = playerController.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f));  //casts ray from the center of the screen
        ray.direction += new Vector3(Random.Range(-spread,spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
        ray.origin = playerController.camTransform.position;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            //item info class does not have the damage info,so cast iteminfo class to weaponinfo class to access damage variable
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((WeaponInfo)itemInfo).damage);
            bool isPlayer = false;
            if (hit.collider.gameObject.name == "Player Controller(Clone)")
            {
                isPlayer = true;
            }
            view.RPC(nameof(RPC_Shoot), RpcTarget.All, hit.point, hit.normal, isPlayer);
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal, bool isPlayer)
    {

        Collider[] colliders = Physics.OverlapSphere(hitPosition, .3f);
        if(colliders.Length != 0)
        {
            GameObject bullet;
            if (isPlayer)
            {
                bullet = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * .001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            }
            else
            {
                bullet = Instantiate(bulletMissPrefab, hitPosition + hitNormal * .001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletMissPrefab.transform.rotation);
            }
            Destroy(bullet, 2.5f);
            bullet.transform.SetParent(colliders[0].transform);
        }
        AudioSource.PlayClipAtPoint(gunShot, transform.position);
    }

    public void SetPlayerController(PlayerController player)
    {
        playerController = player;
    }

}
