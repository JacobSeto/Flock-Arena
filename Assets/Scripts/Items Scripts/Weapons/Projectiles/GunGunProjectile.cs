using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class GunGunProjectile : Projectile
{
    [SerializeField] Camera gunCamera;
    [SerializeField] GameObject cameraHolder;
    [SerializeField] Transform circleReticle;
    Transform playerCamTransform;
    Camera playerCamera;
    PlayerManager playerManager;
    [HideInInspector] public float boostTime;
    [HideInInspector] public float boostStrength;
    private bool boostDone = false;
    [SerializeField] float turnSens;
    public float turnMax;
    [SerializeField] float turnSpeed;

    public SingleShotGun singleShotGun;

    //TODO: add customizaiton to gungun for upgrades

    private void Awake()
    {
        singleShotGun.playerController = playerController;
    }

    public override void Start()
    {
        singleShotGun.spread = singleShotGun.hipSpread;
        base.Start();
        boostTime += Time.time;
        AddSpeed(boostStrength);
        if (view.IsMine)
        {
            playerCamTransform = playerController.camTransform;
            playerCamera = playerController.playerCamera;
            playerController.PlayerCamerasActive(false);
            singleShotGun.playerController = playerController;
            playerManager = playerController.playerManager;
            playerController.camTransform = gunCamera.transform;
            playerController.playerCamera = gunCamera;
            singleShotGun.UpdateItemUI();
            playerController.canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
        else
        {
            Destroy(cameraHolder) ;
        }
    }
    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            rb.velocity = transform.forward * speed;
            GunGunControl();
        }
    }

    private void Update()
    {
        if (view.IsMine)
        {
            GunGunControl();
            //circle reticle follow mouse
            circleReticle.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if(!boostDone && Time.time > boostTime)
        {
            boostDone = true;
            AddSpeed(-boostStrength);
        }

    }

    public void Turn()
    {
        // Get the mouse position in screen space
        Vector3 mousePosition = Input.mousePosition;
        // Divide the mouse position by the screen size
        float rotationX = Mathf.Clamp(-(mousePosition.y / Screen.height -.5f) * turnSens, -turnMax, turnMax) ;
        float rotationY = Mathf.Clamp((mousePosition.x / Screen.width - .5f) * turnSens, -turnMax, turnMax) ;
        // Calculate the rotation delta based on rotation angles and speed
        Vector3 rotationDelta = new Vector3(rotationX, rotationY, 0f) * turnSpeed * Time.deltaTime;
        transform.rotation *= Quaternion.Euler(rotationDelta);
        //remove z rotation
        gameObject.transform.eulerAngles = new Vector3(
                gameObject.transform.eulerAngles.x,
                gameObject.transform.eulerAngles.y,
                0);
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
        
        if (Input.GetMouseButtonDown(1) || Input.GetKey(KeyCode.Escape))
        {
            DestroyProjectile(0);
        }
        Turn();
    }


    private void OnDestroy()
    {
        if (playerController != null)
        {
            gunCamera.gameObject.SetActive(false);
            playerController.camTransform = playerCamTransform;
            playerController.playerCamera = playerCamera;
            singleShotGun.playerController.PlayerCamerasActive(true);
            playerController.canMove = true;
            singleShotGun.canAim = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }
}
