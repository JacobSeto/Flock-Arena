using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLoadout : MonoBehaviourPunCallbacks
{
    [SerializeField] Toggle[] weaponToggles;
    public Dictionary<string,Material> materialDictionary = new Dictionary<string, Material>();
    [SerializeField] Material[] materials;
    [Space]
    [SerializeField] int maxNumWeapons;
    [SerializeField] int maxSkillPoints;
    [SerializeField] int roundSkillPointIncrease;
    int skillPoints;
    [SerializeField] TMP_Text skillPointsText;
    public Dictionary<string, int> skillTier = new Dictionary<string, int>();
    public Skill[] skills;
    string teamName;
    [Space]
    [Header("Tank")]
    [SerializeField] float vital1 = 25f;
    [SerializeField] float vital2 = 40f;
    [Space]
    [Header("Support")]
    [SerializeField] float regen1 = .2f;
    [SerializeField] float regen2 = .15f;
    [Space]
    [Header("Mobility")]
    [SerializeField] float swift1 = 5f;
    [SerializeField] float swift2 = 4f;

    [Space]
    [Header("Utility")]
    [SerializeField] int grenade1 = 3;


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
        if (skills[0].gameObject.GetComponent<Toggle>().isOn)
            playerController.maxHealth += vital1;
        if (skills[1].gameObject.GetComponent<Toggle>().isOn)
            playerController.maxHealth += + vital2;
        if (skills[2].gameObject.GetComponent<Toggle>().isOn)
            playerController.speed += swift1;
        if (skills[3].gameObject.GetComponent<Toggle>().isOn)
            playerController.speed += swift2;
    }


    public int GetWeaponToggleIndex()
    {
        //Make sure that the weapon toggle is the same order as the player controller child gameObjects under the Item Holder GameObject
        int index = 0;
        foreach(Toggle weaponToggle in weaponToggles)
        {
            Debug.Log(index);
            if(weaponToggle.isOn)
                return index;
            index++;
        }
        Debug.Log("Missing Toggle");
        return 0;
    }

}
