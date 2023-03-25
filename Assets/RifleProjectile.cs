using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RifleProjectile : Projectile
{
    [HideInInspector] public Rifle rifle;

    private void OnDestroy()
    {
        if (view.IsMine)
        {
            rifle.RifleActive(true);
        }
    }


}
