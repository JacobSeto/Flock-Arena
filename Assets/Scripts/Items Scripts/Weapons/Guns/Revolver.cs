using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : SingleShotGun
{
    public bool fasterFire { get; set; } = false;

    public void RevolverUpgrades()
    {
        //Checks each revolver upgrade and applies to gun
        if (fasterFire)
        {
            reload = .1f;
        }
    }


}
