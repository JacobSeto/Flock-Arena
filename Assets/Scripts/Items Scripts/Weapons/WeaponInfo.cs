using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Weapon")]

public class WeaponInfo : ItemInfo
{
    public float damage;

    public float fireRate;
    
    //if ammo is -1, unlimited ammo
    public float ammo;

    //if reload is 0, does not reload
    public float reload;

    public bool isAutoFire;  //true if auto, false otherwise

    public float hipSpread; //0 no spread

    public float aimSpread; // no spread

    public float aimSpeed = 3.5f; //speed to move gun into aim

    public float hipSpeed = 3.5f; //speed to move gun out of aim

    public float hipFOV = 60f;

    public float aimFOV = 30f;
}
