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
    public Recoil recoil;
    //Hipfire Recoil
    [SerializeField] private float hipRecoilX;
    [SerializeField] private float hipRecoilY;
    [SerializeField] private float hipRecoilZ;

    //Aim Recoil
    [SerializeField] private float aimRecoilX;
    [SerializeField] private float aimRecoilY;
    [SerializeField] private float aimRecoilZ;

    public bool canAim = true;
    public bool isAiming;
    float specialTime;
    bool specialActive;
    //default value is 0, except some which 0 is valid
    [Header("Weapon Stats")]
    [HideInInspector] public float spread;
    public float damage;
    public float specialCooldown;
    public float hipSpread;
    public float hipSpeed;
    public float aimSpread;
    public float aimSpeed;
    public bool isAutoFire;

    public override void Update()
    {
        if (!view.IsMine)
        {
            return;
        }
        base.Update();
        CheckSpecial();
    }
    public virtual void FixedUpdate()
    {
        if (!view.IsMine)
        {
            return;
        }
        if (!playerController.isPaused)
            Aim();
    }

    public override void ItemInactive()
    {
        if (!view.IsMine)
        {
            return;
        }
        base.ItemInactive();
        if (transform.position != hipPosition.position)
        {
            transform.position = hipPosition.position;
        }
    }

    public override void UseItem()
    {
        Shoot();
    }

    public override void CheckUse()
    {
        base.CheckUse();
        if (useItem&&!playerController.isPaused && isAutoFire && Input.GetMouseButton(0))
        {
            Use();
        }
    }

    public virtual void Shoot()
    {
        //IMPORTANT:  Call the base method AFTER firing.  This is to avoid random movement before the bullet is fired


        //play weapon audio
        //weaponAudio[UnityEngine.Random.Range(0, weaponAudio.Length - 1)].Play();
        transform.position += new Vector3(UnityEngine.Random.Range(-moveX, moveX), UnityEngine.Random.Range(-moveY, moveY), UnityEngine.Random.Range(-moveZ, moveZ));
        transform.Rotate(rotateX, rotateY, rotateZ);
        //vertical rotation for non-autofire weapons
        if (isAiming)
        {
            recoil.RecoilFire(aimRecoilX, aimRecoilY, aimRecoilZ);
        }
        else
        {
            recoil.RecoilFire(hipRecoilX, hipRecoilY, hipRecoilZ);
        }

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
        if (Input.GetKeyDown(KeyCode.Q) && specialActive && !playerController.isPaused)
        {
            Special();
            specialActive = false;
            specialTime = specialCooldown;
        }
    }

    public virtual void Aim()
    {
        if (canAim && playerController.canMove && Input.GetKey(KeyCode.Mouse1))
        {
            isAiming = true;
            AimPosition();
        }
        else
        {
            isAiming = false;
            HipPosition();
        }
    }

    public virtual void HipPosition()
    {
        transform.position = Vector3.Lerp(transform.position, hipPosition.position, hipSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, hipPosition.rotation, hipSpeed * Time.deltaTime);
        SetFieldOfView(Mathf.Lerp(playerController.playerCamera.fieldOfView, ((WeaponInfo)itemInfo).hipFOV, hipSpeed * Time.deltaTime));
        spread = Mathf.Lerp(spread, hipSpread, hipSpeed * Time.deltaTime);
    }

    public virtual void AimPosition()
    {
        transform.position = Vector3.Lerp(transform.position, aimPosition.position, aimSpeed * Time.deltaTime);
        SetFieldOfView(Mathf.Lerp(playerController.playerCamera.fieldOfView, ((WeaponInfo)itemInfo).aimFOV, aimSpeed * Time.deltaTime));
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
