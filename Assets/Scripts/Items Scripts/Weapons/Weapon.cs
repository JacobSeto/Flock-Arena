using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Weapon : Item
{
    [SerializeField] AudioSource[] weaponAudio;

    //Adds shake effect when weapons are used
    [SerializeField] float moveX;
    [SerializeField] float moveY;
    [SerializeField] float moveZ;
    [SerializeField] float rotateX;
    [SerializeField] float rotateY;
    [SerializeField] float rotateZ;

    public bool canAim = true;
    [HideInInspector] public bool canShoot = true;
    [HideInInspector] public float nextShot;
    [HideInInspector] public bool reloading = false;
    [HideInInspector] public float spread;
    float reloadTime;
    float specialTime;
    bool specialActive;
    float maxAmmo;
    //default value is 0, except some which 0 is valid
    [HideInInspector] public float damage;
    [HideInInspector] public float fireRate;
    [HideInInspector] public float reload;
    [HideInInspector] public float specialCooldown;
    [HideInInspector] public float hipSpread = -1;
    [HideInInspector] public float aimSpread = -1;
    [HideInInspector] public float aimSpeed = -1;
    [HideInInspector] public float ammo;
    [HideInInspector] public bool isAutoFire;

    public void SetWeapon()
    {
        //sets weapon stats if not already set by PlayerLoadout
        if (damage == 0)
            damage = ((WeaponInfo)itemInfo).damage;
        if (fireRate == 0)
            fireRate = ((WeaponInfo)itemInfo).fireRate;
        if (reload == 0)
            reload = ((WeaponInfo)itemInfo).reload;
        if (specialCooldown == 0)
            specialCooldown = ((WeaponInfo)itemInfo).specialCooldown;
        if (hipSpread == -1)
            hipSpread = ((WeaponInfo)itemInfo).hipSpread;
        if (aimSpread == -1)
            aimSpread = ((WeaponInfo)itemInfo).aimSpread;
        if (aimSpeed == -1)
            aimSpeed = ((WeaponInfo)itemInfo).aimSpeed;
        if (isAutoFire == false)
            isAutoFire = ((WeaponInfo)itemInfo).isAutoFire;
        if (ammo == 0)
            ammo = ((WeaponInfo)itemInfo).ammo;
        maxAmmo = ammo;
    }

    public virtual void Update()
    {
        if (!view.IsMine)
        {
            return;
        }
        CheckUse();
        CheckSpecial();
        CheckReload();
        UpdateItemUI();
    }
    public virtual void FixedUpdate()
    {
        if (!view.IsMine)
        {
            return;
        }
        Aim();
    }

    public override void Awake()
    {
        base.Awake();
        SetWeapon();
        view = GetComponent<PhotonView>();
        
    }

    public override void ItemActive()
    {
        itemGameObject.SetActive(true);
    }

    public override void ItemInactive()
    {
        itemGameObject.SetActive(false);
        if(transform.position != hipPosition.position)
        {
            transform.position = hipPosition.position;
        }
    }
    
    public override void Use()
    {
        if (canShoot && ammo != 0)
        {
            Shoot();
            nextShot = Time.time + fireRate;
            canShoot = false;
            if (ammo != -1)
            {
                ammo--;
                Reload();
            }
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

    public virtual void CheckUse()
    {
        if (Time.time >= nextShot && canShoot == false)
        {
            canShoot = true;
        }
        if (!playerController.isPaused && isAutoFire && Input.GetMouseButton(0))
        {
            Use();
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

    public override void Reload()
    {
        if(ammo != -1)
        {
            reloadTime = reload+Time.time;
            reloading = true;
        }
    }

    public virtual void CheckReload()
    {
        if (reloading)
        {
            if(reloadTime <= Time.time)
            {
                ammo = maxAmmo;
                reloading = false;
            }

        }
    }

    public override void UpdateItemUI()
    {
        //ammoBoxText
        if (toggleInactive)
        {
            playerController.ammoText.text = "XXX";
        }
        else
        {
            playerController.ammoText.text = ammo.ToString()+ "/" + maxAmmo.ToString();
        }
        //reload bar
        if (reloading)
        {
            playerController.reloadBar.fillAmount = reloadTime - Time.time > .2 ? 1 - (reloadTime - Time.time) / reload : 0;

        }
        else if (playerController.reloadBar.fillAmount != 0)
            playerController.reloadBar.fillAmount = 0;
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
