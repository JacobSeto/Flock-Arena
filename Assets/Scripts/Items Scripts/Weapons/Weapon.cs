using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Weapon : Item
{
    [SerializeField] AudioSource[] weaponAudio;

    //Adds shake effect when weapons are used
    [Header("Weapon Shake")]
    [SerializeField] float moveX;
    [SerializeField] float moveY;
    [SerializeField] float moveZ;
    [SerializeField] float rotateX;
    [SerializeField] float rotateY;
    [SerializeField] float rotateZ;

    public bool canAim = true;
    float specialTime;
    bool specialActive;
    //default value is 0, except some which 0 is valid
    [Header("Weapon Stats")]
    [HideInInspector] public float spread;
    public float damage;
    public float fireRate;
    public float specialCooldown;
    public float hipSpread = -1;
    public float aimSpread = -1;
    public float aimSpeed = -1;
    public bool isAutoFire;
    public override void Update()
    {
        base.Update();
        CheckSpecial();
    }
    public virtual void FixedUpdate()
    {
        if (!view.IsMine)
        {
            return;
        }
        Aim();
    }

    public override void ItemInactive()
    {
        base.ItemInactive();
        if (transform.position != hipPosition.position)
        {
            transform.position = hipPosition.position;
        }
    }

    public override void Use()
    {
        if (useItem && ammo != 0)
        {
            Shoot();
            nextShot = Time.time + fireRate;
            useItem = false;
            if (ammo != -1)
            {
                ammo--;
                Reload();
            }
        }
    }

    public override void CheckUse()
    {
        base.CheckUse();
        if (!playerController.isPaused && isAutoFire && Input.GetMouseButton(0))
        {
            Use();
        }
    }

    public virtual void Shoot()
    {
        //play weapon audio
        //weaponAudio[UnityEngine.Random.Range(0, weaponAudio.Length - 1)].Play();
        transform.position += new Vector3(UnityEngine.Random.Range(-moveX, moveX), UnityEngine.Random.Range(-moveY, moveY), UnityEngine.Random.Range(-moveZ, moveZ));
        transform.Rotate(rotateX, rotateY, rotateZ);
    }
    public virtual void Special() { }

    public virtual void CheckSpecial()
    {
        if (!specialActive && specialTime <= 0)
        {
            specialActive = true;
        }
        if(specialTime > 0)
        {
            specialTime -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Q) && specialActive)
        {
            Special();
            specialActive = false;
            specialTime = specialCooldown;
        }
    }

    public virtual void Aim()
    {
        if (!canAim || !playerController.canMove)
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
        spread = Mathf.Lerp(spread, hipSpread, ((WeaponInfo)itemInfo).hipSpeed * Time.deltaTime);
    }

    public virtual void AimPosition()
    {
        transform.position = Vector3.Lerp(transform.position, aimPosition.position, aimSpeed * Time.deltaTime);
        SetFieldOfView(Mathf.Lerp(playerController.playerCamera.fieldOfView, ((WeaponInfo)itemInfo).aimFOV, ((WeaponInfo)itemInfo).aimSpeed * Time.deltaTime));
        spread = Mathf.Lerp(spread, aimSpread, aimSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, aimPosition.rotation, aimSpeed * Time.deltaTime);
    }

    public override void UpdateItemUI()
    {
        base.UpdateItemUI();
        //weapon ability
        if (specialActive)
        {
            playerController.specialText.text = "Q";
        }
        else
        {
            playerController.specialText.text = (Mathf.CeilToInt(specialTime)).ToString();
        }
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
