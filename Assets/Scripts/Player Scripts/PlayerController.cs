using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [Header("UI")]
    [SerializeField] GameObject cameras;
    [SerializeField] GameObject UI;
    [SerializeField] Image healthbarImage;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text boostText;
    [SerializeField] GameObject cameraHolder;

    public Camera playerCamera { get; set; }
    [SerializeField] Camera playerCameraRef;
    public Transform camTransform { get; set; }
    [SerializeField] Transform camTransformRef;
    public Camera itemCamera { get; set; }
    [SerializeField] Camera itemCameraRef;
    public float mouseSens { get; set; }

    [Space]
    [Header("Player Stats")]
    [SerializeField] float speedConst;
    public float speed { get; set; }
    [SerializeField] float baseSpeed = 20f;
    [SerializeField] float sprintMultiplier;
    [SerializeField] float jumpHeight;
    [SerializeField] float jumpCooldown;
    [SerializeField] float healthRegen;
    [SerializeField] float regenTime;
    [SerializeField] float damageWaitTime;

    public float maxHealth { get; set; }
    [SerializeField] float baseHealth = 200f;
    float jumpCooldownTime = 0;
    public bool canMove { get; set; } = true;
    float nextRegen;
    [SerializeField] float boostTime;
    [SerializeField] float horizontalBoost;
    [SerializeField] float verticalBoost;
    float nextBoost;
    float currentHealth;

    [Space]
    [Header("Items")]
    [SerializeField] Transform itemHolder;
    [SerializeField] List<Item> items;
    int itemIndex = 0;
    int previousItemIndex = -1;

    [Space]
    [Header("Player Controller")]
    float verticalLookRotation;
    bool isGrounded;
    public Transform playerTransform { get; set; }

    public Rigidbody rb { get; set; }
    [SerializeField] Rigidbody rbRef;
    public PhotonView view { get; set; }
    [SerializeField] PhotonView viewRef;
    PlayerManager playerManager;
    public bool isPaused { get; set; } = false;

    private void Awake()
    {
        view = viewRef;
    }


    private void Start()
    {
        
        if (view.IsMine)
        {
            PlayerSetters();
            Cursor.lockState = CursorLockMode.Locked;
            playerManager.SliderMouseSensitivity();
        }
        else
        {
            Destroy(cameras);
            Destroy(rb);
            Destroy(UI);
        }
        EquipItem(itemIndex);
    }
    public void PlayerSetters()
    {
        speed = baseSpeed;
        maxHealth = baseHealth;
        playerTransform = transform;
        playerCamera = playerCameraRef;
        camTransform = camTransformRef;
        itemCamera = itemCameraRef;
        rb = rbRef;
        playerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerManager>();
        SetLoadout(playerManager.GetPlayerLoadout());
        currentHealth = maxHealth;
        healthText.text = currentHealth.ToString("000");
    }
    public void SetLoadout(PlayerLoadout playerLoadout)
    {
        //set team color
        gameObject.GetComponent<Renderer>().sharedMaterial = playerLoadout.GetMaterial();
        //set skill tree skills
        playerLoadout.SkillTree(gameObject.GetComponent<PlayerController>());
        //set player weapon by destroying all other weapons in itemholder
        view.RPC(nameof(RPC_SetWeapon), RpcTarget.All, playerLoadout.GetWeaponToggleIndex());
        //set player weapon skills

        //set item refrences
    }
    [PunRPC]
    public void RPC_SetWeapon(int index)
    {
        for (int i = 0; i < index; i++)
        {
            items.Remove(items[0]);
        }
        while (items.Count > 1)
        {
            items.Remove(items[1]);
        }
    }

    private void Update()
    {
        if (!view.IsMine)
        {
            return;
        }
        Inputs();

        if (Time.time > nextRegen && currentHealth != maxHealth)
        {
            Heal(healthRegen);
        }


        if (transform.position.y < -10f) //Die under -10 y position
        {
            Die();
        }

    }

    private void FixedUpdate()
    {
        if (!view.IsMine)
        {
            return;
        }
        RBMovement();
    }

    public void PlayerCamerasActive(bool isActive)
    {
        cameras.SetActive(isActive);
    }
    public void SetAmmoText(string ammo, string maxAmmo, bool reloading)
    {
        if (!ammoText.gameObject.activeSelf)
            ammoText.gameObject.SetActive(true);
        if(ammo == "-1")
            ammoText.gameObject.SetActive(false);
        if (reloading)
        {
            ammoText.text = "reloading";
        }
        else
        {
            ammoText.text = ammo + '/' + maxAmmo;
        }
    }

    public void AddPlayerForce(Vector3 force)
    {
        if(rb != null)
            rb.AddForce(force);
    }


    private void MouseLook()
    {
        //horizontal rotation
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSens);

        //vertical rotation
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSens;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    private void Move()
    {
        Vector3 moveDirection = transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.AddForce(moveDirection.normalized * speed * sprintMultiplier, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);

        }

        // Vector3 moveDir = new Vector3(-Input.GetAxisRaw("Horizontal"), 0, -Input.GetAxisRaw("Vertical")).normalized;
        //moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);

    }

    private void MoveConstant()
    {
        //set rb velocity for snappy movement
        if (Input.GetKey(KeyCode.W))
            rb.position += transform.forward * speedConst;
        else if (Input.GetKey(KeyCode.S))
            rb.position += -transform.forward * speedConst;
        if (Input.GetKey(KeyCode.D))
            rb.position += transform.right * speedConst;
        if (Input.GetKey(KeyCode.A))
            rb.position += -transform.right * speedConst;
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && jumpCooldownTime <= Time.time)
        {
            rb.velocity += transform.up * jumpHeight;
            jumpCooldownTime = Time.time + jumpCooldown;
        }
    }


    public void GunControl()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            if(itemIndex >= items.Count - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }  
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Count-1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            items[itemIndex].Reload();
        }
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);
        items[itemIndex].UpdateAmmo();

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].ItemNotActive();
            items[previousItemIndex].itemGameObject.SetActive(false);
        }
        previousItemIndex = itemIndex;
        view.RPC(nameof(EnemyEquip), RpcTarget.Others, itemIndex);


    }
    [PunRPC]
    public void EnemyEquip(int _index)
    {
        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }
        previousItemIndex = itemIndex;
    }

    void Boost()
    {
        if (Time.time >= nextBoost)
        {
            boostText.text = "E";
            if (Input.GetKeyDown(KeyCode.E))
            {
                rb.AddForce(cameraHolder.transform.forward * horizontalBoost + transform.up * verticalBoost, ForceMode.Impulse);
                nextBoost = Time.time + boostTime;
            }
        }
        else
        {
            boostText.text = ((int)(1 + nextBoost - Time.time)).ToString();
        }
    }

    public void Inputs()
    {
        if (!canMove)
            return;
        MouseLook();
        GunControl();
        Jump();
        Boost();
    }
    public void RBMovement()
    {
        if (!canMove)
            return;
        Move();
        MoveConstant();
    }

    public void SetGroundedState(bool _grounded)
    {
        isGrounded = _grounded;
    }

    public void TakeDamage(float damage)
    {
        view.RPC(nameof(RPC_TakeDamage),view.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

        healthText.text = currentHealth.ToString("000");

        nextRegen = Time.time + damageWaitTime;
        if(currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    public void Heal(float heal)
    {
        view.RPC(nameof(RPC_Heal), view.Owner, heal);
    }

    [PunRPC]
    void RPC_Heal(float heal, PhotonMessageInfo info)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        healthbarImage.fillAmount = currentHealth / maxHealth;
        healthText.text = currentHealth.ToString("000");
        nextRegen = Time.time + regenTime;
    }

    void Die()
    {
        if(view.IsMine)
            Cursor.lockState = CursorLockMode.None;
        playerManager.Die();
    }

}
