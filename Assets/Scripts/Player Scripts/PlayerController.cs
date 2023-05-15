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
    public GameObject playerUI;
    [SerializeField] Image healthbarImage;
    public Image reloadBar;
    [SerializeField] TMP_Text healthText;
    public TMP_Text ammoText;
    public TMP_Text specialText;
    [SerializeField] TMP_Text boostText;
    [SerializeField] GameObject cameraHolder;
    [SerializeField] CanvasGroup hitUI;
    [SerializeField] Transform damageDisplay;
    [SerializeField] GameObject damageDisplayPrefab;
    [Space]
    public Camera playerCamera;
    public Transform camTransform;
    public Camera itemCamera;
    public float mouseSens;
    [SerializeField] float hitRecoverySpeed;  //how fast it takes for hitUI to become transparent again

    [Space]
    [Header("Player Stats")]
    [SerializeField] float groundDrag;
    [SerializeField] float airDrag;
    [SerializeField] float airMaxSpeed;
    [SerializeField] float maxYVelocity;
    [SerializeField] float moveForceConstant;
    public float walkSpeed;
    public float sprintSpeed;
    public float flockSpeed;
    [HideInInspector] public float flockTime;  //num seconds of airtime, decrements over time
    float speed;
    public MovementState moveState;
    public enum MovementState
    {
        walking,
        sprinting,
        air
    }
    public float jumpHeight;
    public float healthRegen;
    public float regenTime;
    public float damageWaitTime;

    public float maxHealth;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool isPaused = false;
    float nextRegen;
    public float boostCooldown;
    public float boostAirTime;
    [SerializeField] float horizontalBoost;
    [SerializeField] float verticalBoost;
    float nextBoost;
    [HideInInspector] public float currentHealth;

    [Space]
    [Header("Items")]
    [SerializeField] Transform itemHolder;
    [SerializeField] List<Item> items;
    int itemIndex = 0;
    int previousItemIndex = -1;

    [Space]
    [Header("Player Controller")]
    public float verticalLookRotation;
    bool isGrounded;
    public Transform playerTransform;

    public Rigidbody rb;
    public PhotonView view;
    public PlayerManager playerManager;


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
            Destroy(playerUI);
        }
        EquipItem(0);
    }
    public void PlayerSetters()
    {
        playerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerManager>();
        SetLoadout(playerManager.playerLoadout);
        currentHealth = maxHealth;
        healthText.text = currentHealth.ToString("000");
    }
    public void SetLoadout(PlayerLoadout playerLoadout)
    {
        //set team color
        //gameObject.GetComponent<Renderer>().sharedMaterial = playerLoadout.GetMaterial();
       
        //set skill tree skills
        playerLoadout.SkillTree(this);
        //set player weapon by destroying all other weapons in itemholder
        view.RPC(nameof(RPC_SetWeapon), RpcTarget.All, playerLoadout.weaponsSelected);
        //set weapon upgrades
        foreach(Item weaponItem in items)
        {
            playerLoadout.WeaponUpgrades(weaponItem);
        }
        //set player weapon skills

        //set item refrences
    }
    [PunRPC]
    public void RPC_SetWeapon(int[] weaponsSelected)
    {
        //adds weapons selected from itemholder to items List
        foreach(int weaponIndex in weaponsSelected)
        {
            items.Add(itemHolder.GetChild(weaponIndex).gameObject.GetComponent<Item>());
        }
    }

    private void Update()
    {
        if (!view.IsMine)
        {
            return;
        }
        Inputs();
        UpdateMoveState();
        UpdateAirTime();
        UpdateHitAlpha();

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
        Movement();
    }

    public void PlayerCamerasActive(bool isActive)
    {
        cameras.SetActive(isActive);
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

        camTransform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    private void UpdateMoveState()
    {
        //Mode Sprinting
        if(flockTime != 0)
        {
            moveState = MovementState.air;
            speed = flockSpeed;
        }
        else if(Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Mouse1))
        {
            moveState = MovementState.sprinting;
            speed = sprintSpeed;
        }
        else
        {
            moveState = MovementState.walking;
            speed = walkSpeed;
        }
    }

    private void Move()
    {
        Vector3 moveDirection = playerTransform.forward * Input.GetAxisRaw("Vertical") + playerTransform.right * Input.GetAxisRaw("Horizontal");
        rb.AddForce(moveDirection.normalized * speed * moveForceConstant, ForceMode.Force);
    }

    private void SpeedControl()
    {
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = airDrag;
        Vector3 flatVect = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(moveState == MovementState.air)
        {
            if(flatVect.magnitude > airMaxSpeed)
            {
                Vector3 limitVect = flatVect.normalized * airMaxSpeed;
                rb.velocity = new Vector3(limitVect.x, rb.velocity.y, limitVect.z);
            }
        }
        else if (flatVect.magnitude > speed)
        {
            Vector3 limitVect = flatVect.normalized * speed;
            rb.velocity = new Vector3(limitVect.x, rb.velocity.y, limitVect.z);
        }
        if (rb.velocity.y > maxYVelocity)
            rb.velocity = new Vector3(rb.velocity.x, maxYVelocity, rb.velocity.z);
    }

    public void UpdateAirTime()
    {
        //decrements airTime
        if (flockTime == 0)
            return;
        flockTime -= Time.deltaTime;
        if(flockTime <= 0 || isGrounded)
        {
            flockTime = 0;
        }

    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(playerTransform.up * jumpHeight, ForceMode.Impulse);
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
        if (!canMove)
            return;
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
        items[itemIndex].ItemSetActive(true);
        items[itemIndex].isEquip = true;
        if (previousItemIndex != -1)
        {
            items[previousItemIndex].ItemInactive();
            items[previousItemIndex].isEquip = false;
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
                flockTime += boostAirTime;
                rb.AddForce(cameraHolder.transform.forward * horizontalBoost + playerTransform.up * verticalBoost, ForceMode.Impulse);
                nextBoost = Time.time + boostCooldown;
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
    public void Movement()
    {
        if (!canMove)
            return;
        Move();
        SpeedControl();
    }

    public void SetGroundedState(bool _grounded)
    {
        isGrounded = _grounded;
    }

    public Transform DamageTransform()
    {
        return playerTransform;
    }

    public void TakeDamage(float damage)
    {
        if (damage == 0)
        {
            return;
        }
        view.RPC(nameof(RPC_TakeDamage),view.Owner, damage);
        view.RPC(nameof(RPC_DisplayDamage), RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        hitUI.alpha += damage/maxHealth;
        healthbarImage.fillAmount = currentHealth / maxHealth;

        healthText.text = currentHealth.ToString("000");

        nextRegen = Time.time + damageWaitTime;
        if(currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    [PunRPC]
    void RPC_DisplayDamage(float damage)
    {
        //display damage on other players' screens
        GameObject damageGameObject = Instantiate(damageDisplayPrefab, damageDisplay.position, damageDisplay.rotation, damageDisplay);
        damageGameObject.GetComponentInChildren<TMP_Text>().text = damage.ToString();
        Destroy(damageGameObject, 1);
    }

    public void UpdateHitAlpha()
    {
        //Lerp alpha of hitUI to transition back to 0 alpha
        if(hitUI.alpha != 0)
            hitUI.alpha = Mathf.Lerp(hitUI.alpha, 0f, hitRecoverySpeed * Time.deltaTime);
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

    public void Die()
    {
        if(view.IsMine)
            Cursor.lockState = CursorLockMode.None;
        playerManager.Die();
    }

}
