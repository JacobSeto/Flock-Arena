using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLoadout : MonoBehaviourPunCallbacks
{
    [SerializeField] Toggle[] weaponToggles;
    [SerializeField] TMP_Text[] orderText;
    public int[] weaponsSelected;  //indexes initially selected
    int previousSelected = 0;
    public Skill[] skills;
    public int maxGunUpgrades;
    public int numGunUpgrades;
    [Space]
    public Dictionary<string,Material> materialDictionary = new Dictionary<string, Material>();
    [SerializeField] Material[] materials;
    [Space]
    [SerializeField] int maxSkillPoints;
    [SerializeField] int roundSkillPointIncrease;
    int skillPoints;
    [SerializeField] TMP_Text skillPointsText;
    public Dictionary<string, int> skillTier = new Dictionary<string, int>();
    string teamName = "blue";
    [Space]
    [Header("Tank")]
    [SerializeField] float vital1 = 25f;
    [SerializeField] float vital2 = 40f;
    [Space]
    [Header("Support")]
    [SerializeField] float regen1 = .3f;
    [SerializeField] float regen2 = .2f;
    [Space]
    [Header("Mobility")]
    [SerializeField] float swift1 = 1.2f;
    [SerializeField] float swift2 = 1.2f;

    [Space]
    [Header("Utility")]
    [SerializeField] int medkit1;
    [SerializeField] int grenade1;

    //Revolver
    [Header("Revolver Path 1")]
    [SerializeField] float heavyChange;  //increase damage of coin hitting players directly
    [SerializeField] float moneyHandsCooldown;  //shorter coin cooldown
    [SerializeField] float silverDollarMultiplyer;  //coin deflect multipler increased
    [SerializeField] int titaniumHealth;  //increase health of coin to be shot multiple times

    [Header("Revolver Path 2")]
    [SerializeField] float betterBarrelReloadTime; //faster reload time
    [SerializeField] float quickDrawTime;  // aim-in much faster
    //Explosive Round damage for the explosive round
    [SerializeField] float revautoFirerate; //revauto:  auto fire enabled and faster firerate


    [Header("Revolver Path 3")]
    [SerializeField] float largerMagazines;  //increase the ammo capacity of both revolver and rifle.  multiply ammo
    [SerializeField] int numFrag;  //when shotgun shoots coin, the coin fragments and shoots multiple projectiles
    [SerializeField] float oiledBulletsDamageBonus;  //RPG projectiles shot by revolver do extra explosion damage
    [SerializeField] float recursiveCoinRadiusBonus; //When coin hits recursive round (not projectile), increase explosion radius






    [Space]
    public GameObject SpawnButton;
    public TMP_Text StartCountdown;

    private void Awake()
    {
        skillPoints = maxSkillPoints;
        SetMaterialDictionary();
        SetSkillTierDictionary();
    }
    private void Start()
    {
        UpdateSkillTree();
    }

    public void SetSkillPointsText()
    {
        skillPointsText.text = "Skill Points: " + skillPoints.ToString();
    }

    public void SetMaterialDictionary()
    {
        //team colors
        materialDictionary["red"] = materials[0];
        materialDictionary["blue"] = materials[1];
    }

    public void SetSkillTierDictionary()
    {
        //skill paths and tiers
        skillTier["support"] = 0;
        skillTier["tank"] = 0;
        skillTier["mobility"] = 0;
        skillTier["utility"] = 0;
        //upgrade paths for weapons

        //tiers for revolver
        skillTier["revolver 1"] = 0;
        skillTier["revolver 2"] = 0;
        skillTier["revolver 3"] = 0;
        //tiers for rifle
        skillTier["rifle 1"] = 0;
        skillTier["rifle 2"] = 0;
        skillTier["rifle 3"] = 0;
        //tiers for shotgun
        skillTier["shotgun 1"] = 0;
        skillTier["shotgun 2"] = 0;
        skillTier["shotgun 3"] = 0;
        //tiers for rpg
        skillTier["rpg 1"] = 0;
        skillTier["rpg 2"] = 0;
        skillTier["rpg 3"] = 0;
        //tiers for gungun
        skillTier["gungun 1"] = 0;
        skillTier["gungun 2"] = 0;
        skillTier["gungun 3"] = 0;
        //tiers for katana
        skillTier["katana 1"] = 0;
        skillTier["katana 2"] = 0;
        skillTier["katana 3"] = 0;
        //tiers for recursive shot
        skillTier["recursive 1"] = 0;
        skillTier["recursive 2"] = 0;
        skillTier["recursive 3"] = 0;
    }

    public void SetSkillTier(string skillPath, int tier)
    {
        skillTier[skillPath] = tier;
        print(skillTier[skillPath]);
    }

    public int GetSkillTier(string skillPath)
    {
        return skillTier[skillPath];
    }

    public int GetSkillPoints()
    {
        return skillPoints;
    }

    public void SpendSkillPoints(int cost)
    {
        skillPoints -= cost;
        UpdateSkillTree();
    }

    public void ResetPoints()
    {
        skillPoints = maxSkillPoints;
        foreach (Skill skill in skills)
            skill.GetComponent<Toggle>().isOn = false;
        numGunUpgrades = 0;
        SetSkillTierDictionary();
        UpdateSkillTree();
    }

    public void AddPoints(int points)
    {
        skillPoints += points;
        maxSkillPoints += points;
    }

    public void SetTeam(string team)
    {
        teamName = team;
    }

    public string Team()
    {
        return teamName;
    }
    public Material GetMaterial()
    {
        return materialDictionary[teamName];
    }

    public void UpdateSkillTree()
    {
        print("check");
        //sets toggles interactable if conditions are met
        foreach (Skill skill in skills)
            skill.CheckInteractable();
        SetSkillPointsText();
    }

    public void SkillTree(PlayerController playerController)
    {
        //Checks each toggle in every category if selected to apply the skill to player
        Tank(playerController);
        Support(playerController);
        Mobility(playerController);
        Utility(playerController);

     
    }

    public void Tank(PlayerController playerController)
    {
        //handles all tank skills.  Index 0-7
        if (skills[0].gameObject.GetComponent<Toggle>().isOn)
            playerController.maxHealth += vital1;
        if (skills[2].gameObject.GetComponent<Toggle>().isOn)
            playerController.maxHealth += +vital2;
    }
    public void Support(PlayerController playerController)
    {
        //handles all tank skills.  Index 8-15
        if (skills[8].gameObject.GetComponent<Toggle>().isOn)
            playerController.regenTime = regen1;
        if (skills[10].gameObject.GetComponent<Toggle>().isOn)
            playerController.regenTime = regen2;
    }
    public void Mobility(PlayerController playerController)
    {
        //handles all tank skills.  Index 16-23
        if (skills[16].gameObject.GetComponent<Toggle>().isOn)
        {
            playerController.walkSpeed *= swift1;
            playerController.sprintSpeed *= swift1;
            playerController.flockSpeed *= swift1;
        }

        if (skills[18].gameObject.GetComponent<Toggle>().isOn)
        {
            playerController.walkSpeed *= swift2;
            playerController.sprintSpeed *= swift2;
            playerController.flockSpeed *= swift2;
        }
    }
    public void Utility(PlayerController playerController)
    {
        //handles all tank skills.  Index 24-31
    }

    public void WeaponUpgrades(Item weapon)
    {
        //applies upgrades to weapon being used.  Checks the name of the weapon, then applies corresponding upgrades

        switch (weapon.itemInfo.itemName)
        {
            case "Revolver":
                Revolver revolver = weapon.gameObject.GetComponent<Revolver>();
                //Path 1
                if (skills[32].gameObject.GetComponent<Toggle>().isOn)
                    revolver.coinDamage += heavyChange;
                if (skills[33].gameObject.GetComponent<Toggle>().isOn)
                    revolver.specialCooldown = moneyHandsCooldown;
                if (skills[34].gameObject.GetComponent<Toggle>().isOn)
                    revolver.deflectMultiplyer = silverDollarMultiplyer;
                if (skills[35].gameObject.GetComponent<Toggle>().isOn)
                    revolver.coinHealth = titaniumHealth;
                //Path 2
                if (skills[36].gameObject.GetComponent<Toggle>().isOn)
                    revolver.reload = betterBarrelReloadTime;
                if (skills[37].gameObject.GetComponent<Toggle>().isOn)
                    revolver.aimSpeed = quickDrawTime;
                if (skills[38].gameObject.GetComponent<Toggle>().isOn)
                    revolver.explosionRounds = true;
                if (skills[39].gameObject.GetComponent<Toggle>().isOn)
                {
                    revolver.isAutoFire = true;
                    revolver.fireRate = revautoFirerate;
                }
                break;
            case "Rifle":
                Debug.Log("rifle");
                break;

            default:
            Debug.Log("weapon does not exist");
            break;
        }
    }

    public void UpdateWeaponToggles(int index)
    {
        //if selected, update
        print("update weapon toggles");
        if (weaponToggles[index].isOn && weaponToggles[index].gameObject.activeInHierarchy)
        {
            weaponsSelected[0] = weaponsSelected[1];
            weaponsSelected[1] = index;
            weaponToggles[previousSelected].isOn = false;
            //set order text
            orderText[previousSelected].text = "";
            orderText[weaponsSelected[0]].text = "1";
            orderText[weaponsSelected[1]].text = "2";

            previousSelected = weaponsSelected[0];
        }       
    }

    public void RoundSkillIncrease()
    {
        maxSkillPoints += roundSkillPointIncrease;
        skillPoints += roundSkillPointIncrease;
        SetSkillPointsText();
        UpdateSkillTree();
    }

}
