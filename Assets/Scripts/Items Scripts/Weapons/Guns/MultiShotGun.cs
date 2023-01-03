using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiShotGun : Weapon
{
    PhotonView view;

    GameObject bulletType;

    [SerializeField] int numBullets;
    [SerializeField] float spreadCo;


    public override void Awake()
    {
        base.Awake();
        view = GetComponent<PhotonView>();
        spread = ((WeaponInfo)itemInfo).hipSpread;
        ammo = ((WeaponInfo)itemInfo).ammo;
        bulletType = bulletMissPrefab;
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
        for (int i = 0; i < numBullets; i++)
        {
            Ray ray = playerController.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f));  //casts ray from the center of the screen
            float mSpread = spread + spreadCo;
            ray.direction += new Vector3(Random.Range(-mSpread, mSpread), Random.Range(-mSpread, mSpread), Random.Range(-mSpread, mSpread));
            ray.origin = playerController.camTransform.position;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //item info class does not have the damage info,so cast iteminfo class to weaponinfo class to access damage variable
                hit.collider.gameObject.GetComponentInParent<IDamageable>()?.TakeDamage(((WeaponInfo)itemInfo).damage);
                bulletType = bulletMissPrefab;
                if (hit.collider.gameObject.name == "Player Controller(Clone)")
                {
                    bulletType = bulletImpactPrefab;
                }
                view.RPC(nameof(RPC_Shoot), RpcTarget.All, hit.point, hit.normal);
            }
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, .3f);
        if (colliders.Length != 0)
        {
            GameObject bullet = Instantiate(bulletMissPrefab, hitPosition + hitNormal * .001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletMissPrefab.transform.rotation);
            Destroy(bullet, 2.5f);
            bullet.transform.SetParent(colliders[0].transform);
        }
    }
}
