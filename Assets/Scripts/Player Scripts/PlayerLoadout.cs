using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLoadout : MonoBehaviourPunCallbacks
{
    Item[] itemLoadout;
    [SerializeField] Toggle[] itemToggles;
    public Dictionary<string,Material> materialDictionary = new Dictionary<string, Material>();
    [SerializeField] Material[] materials;
    [Space]
    [SerializeField] int maxSkillPoints;
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
    [SerializeField] float swift1 = 5f;
    [SerializeField] float swift2 = 4f;
    [Space]
    [Header("Damage")]
    [SerializeField] float damage1;
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
        skillTier["damage"] = 0;
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

    public Item[] GetItemLoadout()
    {
        return itemLoadout;
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

}
