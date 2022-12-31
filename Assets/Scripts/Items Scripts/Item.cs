using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    //true if player holds the item in inventory indefinitely.  False if temporary item that is limited use.
    [SerializeField] bool InInventory;
    public ItemInfo itemInfo;
    public GameObject itemGameObject;
    public PlayerController playerController;


    public virtual void Awake()
    {
        if (!InInventory)
            return;
        GetComponentInParent<PlayerController>();
        ItemOwnership();
    }

    public abstract void ItemNotActive();

    public abstract void Use();

    public abstract void Reload();

    public abstract void UpdateAmmo();

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
