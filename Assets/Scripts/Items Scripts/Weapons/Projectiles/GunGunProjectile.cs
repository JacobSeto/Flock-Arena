using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class GunGunProjectile : Projectile
{
    [SerializeField] GameObject gunCamera;
    [SerializeField] float vSens;
    [SerializeField] float hSens;

    Transform playerCamTransform;
    Camera playerCamera;
    PlayerManager playerManager;
    private bool boostDone = false;

    [SerializeField] SingleShotGun gungun;
    public float boostTime;
    public float boostStrength;

    //TODO: add customizaiton to gungun for upgrades

    public override void Start()
    {
        base.Start();
        boostTime += Time.time;
        AddSpeed(boostStrength);
        if (view.IsMine)
        {
            playerCamTransform = playerController.camTransform;
            playerCamera = playerController.playerCamera;
            playerController.PlayerCamerasActive(false);
            gungun.playerController = playerController;
            playerManager = playerController.playerManager;
            playerController.camTransform = gunCamera.transform;
            playerController.playerCamera = gunCamera.GetComponent<Camera>();
            gungun.UpdateItemUI();
            playerController.canMove = false;
        }
        else
        {
            Destroy(gunCamera);
        }
    }
    private void FixedUpdate()
    {
        if(view.IsMine)
            rb.velocity = transform.forward * speed;
    }

    private void Update()
    {
        if (view.IsMine)
        {
            GunGunControl();
        }

        if(!boostDone && Time.time > boostTime)
        {
            boostDone = true;
            AddSpeed(-boostStrength);
        }
    }

    public void GunGunControl()
    {
        if (!playerManager.inGame)
        {
            PhotonNetwork.Destroy(gameObject);
            return;
        }
        if (!playerManager.isPlayer || playerController.isPaused)
            return;
        if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(vSens, 0,0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(-vSens, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, hSens, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -hSens, 0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Explosion(exploDamage);
        }
        if(transform.rotation.z != 0)
        {
            gameObject.transform.eulerAngles = new Vector3(
                gameObject.transform.eulerAngles.x,
                gameObject.transform.eulerAngles.y,
                0);
        }
    }


    private void OnDestroy()
    {
        if (playerController != null)
        {
            playerController.ammoText.text = "drone";
            gunCamera.SetActive(false);
            playerController.camTransform = playerCamTransform;
            playerController.playerCamera = playerCamera;
            gungun.playerController.PlayerCamerasActive(true);
            playerController.canMove = true;
        }
    }
}
