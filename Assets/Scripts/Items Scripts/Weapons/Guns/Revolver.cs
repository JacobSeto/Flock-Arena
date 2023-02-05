using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : SingleShotGun
{
    [SerializeField] float quickHandReloadTime;
    [HideInInspector] public bool quickHands = false;

    public void RevolverUpgrades()
    {
        //Checks each revolver upgrade and applies to gun
        if (quickHands)
        {
            reload = quickHandReloadTime;
        }
    }

    public override void Special()
    {
        //Revolver Special: Shoot a coin projectile.  When shot,
        //shoot at nearest enemy

    }


}
