using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Item : MonoBehaviourPunCallbacks
{
    [Header("Item Stats")]
    //true if player holds the item in inventory indefinitely.  False if temporary item that is limited use.
    public PhotonView view;
    public bool InInventory;
    public ItemInfo itemInfo;
    public GameObject itemGameObject;
    public bool toggleInactive = false; //checks if item can change from an inactive state
    public bool isEquip; //checks if the item is currently equipped
    public PlayerController playerController;

    [HideInInspector] public bool useItem = true;
    [HideInInspector] public float nextShot;
    [HideInInspector] public bool reloading = false;
    float reloadTime;
    [HideInInspector] public float ammo;
    public float maxAmmo;
    public float reload;


    public virtual void Awake()
    {
        if (!InInventory)
            return;
        GetComponentInParent<PlayerController>();
        ItemOwnership();
        ammo = maxAmmo;
    }
    public virtual void Update()
    {
        if (!view.IsMine)
        {
            return;
        }
        CheckUse();
        CheckReload();
        UpdateItemUI();

    }


    /**
     * Specialized SetActive for Items
     */
    public virtual void ItemSetActive(bool isActive)
    {
        if(!toggleInactive && isActive)
        {
            ItemActive();
        }
        else
        {
            ItemInactive();
        }
    }
    public virtual void ItemActive()
    {
        itemGameObject.SetActive(true);
    }

    public virtual void ItemInactive()
    {
        itemGameObject.SetActive(false);
    }


    public abstract void Use();

    public virtual void CheckUse()
    {
        if (Time.time >= nextShot && useItem == false)
        {
            useItem = true;
        }
    }

    public virtual void Reload()
    {
        if (ammo != -1)
        {
            reloadTime = reload + Time.time;
            reloading = true;
        }
    }

    public virtual void CheckReload()
    {
        if (reloading)
        {
            if (reloadTime <= Time.time)
            {
                ammo = maxAmmo;
                reloading = false;
            }

        }
    }

    public virtual void UpdateItemUI()
    {
        //ammoBoxText
        if (toggleInactive)
        {
            playerController.ammoText.text = "XXX";
        }
        else
        {
            playerController.ammoText.text = ammo.ToString() + "/" + maxAmmo.ToString();
        }
        //reload bar
        if (reloading)
        {
            playerController.reloadBar.fillAmount = reloadTime - Time.time > .2 ? 1 - (reloadTime - Time.time) / reload : 0;

        }
        else if (playerController.reloadBar.fillAmount != 0)
            playerController.reloadBar.fillAmount = 0;
    }

    public void ItemOwnership()
    {
        //checks who owns item to correctly delete corresponding gameobject for layer sorting and game logic
        if (playerController.view.IsMine)
            //Destroy enemy item
            Destroy(transform.GetChild(1).gameObject);
        else
            //destroy player item
            Destroy(transform.GetChild(0).gameObject);
    }
}
