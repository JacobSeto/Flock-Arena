using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MultiShotGun
{
    public float shotgunBlastForce;
    public float blastFlockTime;

    public override void Special()
    {
        //Shotgun Special:  Shotgun blasts player into the air!
        Shoot();
        playerController.flockTime += blastFlockTime;
        playerController.AddPlayerForce(-playerController.cameraTransform.forward * shotgunBlastForce*100);


    }
}
