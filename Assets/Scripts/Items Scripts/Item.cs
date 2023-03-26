using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Item : MonoBehaviourPunCallbacks
{
    //true if player holds the item in inventory indefinitely.  False if temporary item that is limited use.
    public PhotonView view;
    public bool InInventory;
    public ItemInfo itemInfo;
    public GameObject itemGameObject;
    public bool toggleInactive = false; //checks if item can change from an inactive state
    public bool isEquip; //checks if the item is currently equipped
    [HideInInspector] public PlayerController playerController;


    public virtual void Awake()
    {
        if (!InInventory)
            return;
        GetComponentInParent<PlayerController>();
        ItemOwnership();
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
    public abstract void ItemActive();
    public abstract void ItemInactive();


    public abstract void Use();

    public abstract void Reload();

    public abstract void UpdateItemUI();

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
