using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Katana : Weapon
{
    [SerializeField] GameObject chargeUI;
    [SerializeField] Image chargeBar;
    [SerializeField] TMP_Text chargeText;
    [Space]
    [SerializeField] Collider swordCollider;
    [SerializeField] float swingAngleStop;
    [SerializeField] float swingSpeed;
    [SerializeField] float aimCooldown;
    [SerializeField] Transform swing1Start;
    [SerializeField] Transform swing1End;
    [SerializeField] Transform swing2Start;
    [SerializeField] Transform swing2End;
    bool swingleft;
    bool swinging;
    float aimCooldownTime;

    [Space]
    [SerializeField] GameObject slashLinePrefab;
    [SerializeField] GameObject slashHitPrefab;
    [SerializeField] Vector3 slashYOffset;
    [SerializeField] float slashUpdateTime;
    [SerializeField] float maxSlashDistance;
    [SerializeField] float maxSlashDamage;
    [SerializeField] float slashChargeTime;
    [SerializeField] float slashDelay;
    [SerializeField] float slashCoolDownTime;

    bool slashing;
    float slashUpdate;
    GameObject slashLine;
    GameObject slashHit;
    Vector3 slashTeleport;
    float slashPercent;
    float slashDamage;
    float lastSlashDamage;
    float slashDistance;
    float lastSlashDistance;
    float slashCharge;
    float slashCoolDown;
    public List<Transform> swordHit = new List<Transform>();  //lsit of transforms swordHits

    public void KatanaUpgrades()
    {
        //Checks each katana upgrade and applies to gun
    }


    public override void Awake()
    {
        base.Awake();
        view = GetComponent<PhotonView>();
        swordCollider.enabled = false;
        UpdateSlashPercent();
    }

    public override void Update()
    {
        if (!view.IsMine)
        {
            return;
        }
        if (aimCooldownTime <= Time.time)
        {
            canAim = true;
        }
        if (swinging)
            Swinging();
        Aim();
        CheckUse();
        CheckReload();
        ChargeSlash();
    }

    public override void Use()
    {
        if (canShoot && ammo != 0 && !reloading && !slashing)
        {
            Shoot();
            nextShot = Time.time + ((WeaponInfo)itemInfo).fireRate;
            canShoot = false;
            if (ammo != -1)
                ammo--;
            UpdateItemUI();
        }

    }

    public override void Aim()
    {
        if (slashCoolDown > Time.time && !swinging)
            HipPosition();
        else
            base.Aim();
    }

    public void ChargeSlash()
    {
        if (swinging || slashCoolDown > Time.time)
            return;
        //Holding right click charges slash damage and distance.  Teleports to position
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            lastSlashDistance = slashDistance;
            lastSlashDamage = slashDamage;
            Invoke(nameof(Slash), slashDelay);
            slashing = false;
            slashCoolDown = Time.time + slashCoolDownTime;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
            slashing = true;
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (!chargeUI.activeSelf)
                chargeUI.SetActive(true);
            slashCharge += Time.deltaTime;
            if (slashCharge > slashChargeTime)
                slashCharge = slashChargeTime;
            UpdateSlashPercent();
            if(slashUpdate <= Time.time)
            {

                CreateSlashLine();
                slashUpdate = slashUpdateTime + slashUpdateTime;
            }
        }
        else
        {
            if (slashCharge != 0)
            {
                Destroy(slashLine); Destroy(slashHit);
                slashCharge = 0;
                UpdateSlashPercent();
            }
            if (chargeUI.activeSelf)
                chargeUI.SetActive(false);
        }
    }

    public override void Shoot()
    {

        base.Shoot();
        Swing();
    }
    void Swing()
    {
        canAim = false;
        aimCooldownTime =  reload + aimCooldown + Time.time;
        swinging = true;
        swingleft = !swingleft;
        swordCollider.enabled = true;
        if (swingleft)
        {
            transform.rotation = swing1Start.rotation;
            transform.position = swing1Start.position;
        }
        else
        {
            transform.rotation = swing2Start.rotation;
            transform.position = swing2Start.position;
        }
    }

    public void Swinging()
    {
        if (swingleft)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, swing1End.rotation, swingSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, swing1End.rotation) <= swingAngleStop)
            {
                swordCollider.enabled = false;
                swordHit.Clear();
                swinging = false;
            }
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, swing2End.rotation, swingSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, swing2End.rotation) <= swingAngleStop)
            {
                swordCollider.enabled = false;
                swordHit.Clear();
                swinging = false;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (view.IsMine)
        {
            IDamageable colDamage = col.gameObject.GetComponent<IDamageable>();
            if (colDamage != null && !swordHit.Contains(colDamage.DamageTransform()))
            {
                colDamage.TakeDamage(damage);
                swordHit.Add(colDamage.DamageTransform());
            }
        }
    }

    public void Slash()
    {
        Ray slashRay = playerController.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f));
        slashRay.origin = playerController.camTransform.position;
        if (Physics.Raycast(slashRay, out RaycastHit hit, lastSlashDistance))
        {
            hit.collider.gameObject.GetComponentInParent<IDamageable>()?.TakeDamage(lastSlashDamage);
            slashTeleport = hit.point + playerController.playerTransform.up / 2 - playerController.playerTransform.forward;
        }
        else
        {
            slashTeleport = playerController.playerTransform.position + playerController.camTransform.forward * lastSlashDistance;
        }
        playerController.playerTransform.position = slashTeleport;
    }

    public void CreateSlashLine()
    {
        Destroy(slashLine); Destroy(slashHit);
        Vector3 halfPoint = playerController.camTransform.position + playerController.camTransform.forward * slashDistance/2;
        slashLine = Instantiate(slashLinePrefab, halfPoint + slashYOffset, playerController.camTransform.rotation);
        slashLine.transform.localScale = new Vector3(0.1f, 0.1f, slashDistance);
        Ray slashRay = playerController.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f));
        slashRay.origin = playerController.camTransform.position;
        if (Physics.Raycast(slashRay, out RaycastHit hit, slashDistance))
        {
            slashHit = Instantiate(slashHitPrefab, hit.point, Quaternion.identity);
        }
    }

    public void UpdateSlashPercent()
    {
        //only returns in 10 percents
        slashPercent = Mathf.Floor(slashCharge / slashChargeTime * 10) / 10;
        slashDamage = maxSlashDamage * slashPercent;
        slashDistance = maxSlashDistance * slashPercent;
        chargeBar.fillAmount = slashPercent;
        chargeText.text = (slashPercent * 100).ToString() + "%";
    }


    public override void Special()
    {
        //Katana Special:  Shoots a blade beam
    }
    private void OnDestroy()
    {
        Destroy(slashLine); Destroy(slashHit);
    }
}
