using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLoadout : MonoBehaviourPunCallbacks
{
    [SerializeField] Toggle[] weaponToggles;
    public Skill[] skills;
    public Skill[] weaponUpgrades;
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

        skillTier["revolver 1"] = 0;
        skillTier["revolver 2"] = 0;
        skillTier["revolver 3"] = 0;

        skillTier["rifle 1"] = 0;
        skillTier["rifle 2"] = 0;
        skillTier["rifle 3"] = 0;


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
            playerController.airSpeed *= swift1;
        }

        if (skills[18].gameObject.GetComponent<Toggle>().isOn)
        {
            playerController.walkSpeed *= swift2;
            playerController.sprintSpeed *= swift2;
            playerController.airSpeed *= swift2;
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
                Debug.Log("revolver");
                break;

            case "Rifle":
                Debug.Log("rifle");
                break;

            default:
            Debug.Log("weapon does not exist");
            break;
        }
    }

    public int GetWeaponToggleIndex()
    {
        //Make sure that the weapon toggle is the same order as the player controller child gameObjects under the Item Holder GameObject
        int index = 0;
        foreach(Toggle weaponToggle in weaponToggles)
        {
            if(weaponToggle.isOn)
                return index;
            index++;
        }
        Debug.Log("Missing Toggle");
        return 0;
    }

    public void RoundSkillIncrease()
    {
        maxSkillPoints += roundSkillPointIncrease;
        skillPoints += roundSkillPointIncrease;
        SetSkillPointsText();
        UpdateSkillTree();
    }

}
