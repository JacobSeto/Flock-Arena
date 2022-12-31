using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Weapon : Item
{
    protected bool canShoot = true;
    protected bool canAim = true;
    protected float nextShot;
    protected float spread;
    protected float ammo;
    protected float reloadTime;
    protected bool reloading = false;

    public override void ItemNotActive()
    {
        reloading = false;
        if(transform.position != hipPosition.position)
        {
            transform.position = hipPosition.position;
        }
    }
    
    public abstract override void Use();

    public virtual void CheckUse()
    {
        if (Time.time >= nextShot && canShoot == false)
        {
            canShoot = true;
        }
        if (((WeaponInfo)itemInfo).isAutoFire && Input.GetMouseButton(0))
        {
            Use();
        }
    }

    public virtual void Aim()
    {
        if (!canAim)
            return;
        if (Input.GetKey(KeyCode.Mouse1))
        {
            AimPosition();
        }
        else
        {
            HipPosition();
        }
    }

    public virtual void HipPosition()
    {
        transform.position = Vector3.Lerp(transform.position, hipPosition.position, ((WeaponInfo)itemInfo).hipSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, hipPosition.rotation, ((WeaponInfo)itemInfo).hipSpeed * Time.deltaTime);
        SetFieldOfView(Mathf.Lerp(playerController.playerCamera.fieldOfView, ((WeaponInfo)itemInfo).hipFOV, ((WeaponInfo)itemInfo).hipSpeed * Time.deltaTime));
        spread = Mathf.Lerp(spread, ((WeaponInfo)itemInfo).hipSpread, ((WeaponInfo)itemInfo).hipSpeed * Time.deltaTime);
    }

    public virtual void AimPosition()
    {
        transform.position = Vector3.Lerp(transform.position, aimPosition.position, ((WeaponInfo)itemInfo).aimSpeed * Time.deltaTime);
        SetFieldOfView(Mathf.Lerp(playerController.playerCamera.fieldOfView, ((WeaponInfo)itemInfo).aimFOV, ((WeaponInfo)itemInfo).aimSpeed * Time.deltaTime));
        spread = Mathf.Lerp(spread, ((WeaponInfo)itemInfo).aimSpread, ((WeaponInfo)itemInfo).aimSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, aimPosition.rotation, ((WeaponInfo)itemInfo).aimSpeed * Time.deltaTime);
    }

    public override void Reload()
    {
        if (((WeaponInfo)itemInfo).reload != 0 && !reloading && ammo != ((WeaponInfo)itemInfo).ammo)
        {
            reloading = true;
            reloadTime = ((WeaponInfo)itemInfo).reload + Time.time;
            UpdateAmmo();
        }
    }

    public virtual void CheckReload()
    {
        if (reloading && Time.time >= reloadTime)
        {
            ammo = ((WeaponInfo)itemInfo).ammo;
            reloading = false;
            UpdateAmmo();
        }
    }

    public override void UpdateAmmo()
    {
        if (reloading)
        {
            playerController.SetAmmoText("", "", reloading);
        }
        else
        {
            playerController.SetAmmoText(ammo.ToString(), ((WeaponInfo)itemInfo).ammo.ToString(), reloading);
        }
    }

    public virtual float GetDamage()
    {
        return ((WeaponInfo)itemInfo).damage;
    }

    private void SetFieldOfView(float fov)
    {

        playerController.itemCamera.fieldOfView = fov;
        playerController.playerCamera.fieldOfView = fov;
    }

    public GameObject bulletImpactPrefab;

    public GameObject bulletMissPrefab;

    public Transform hipPosition;

    public Transform aimPosition;


}
