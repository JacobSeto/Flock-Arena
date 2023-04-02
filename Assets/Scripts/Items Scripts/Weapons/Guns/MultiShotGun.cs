using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiShotGun : SingleShotGun
{

    [SerializeField] int numBullets;

    public override void Shoot()
    {
        for(int i = 0; i < numBullets; i++)
        {
            base.Shoot();
        }
    }
}
