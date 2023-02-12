using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Skill : MonoBehaviour
{
    [SerializeField] SkillInfo skillInfo;
    [SerializeField] Toggle toggle;
    [SerializeField] TMP_Text skillText;
    [SerializeField] PlayerLoadout playerLoadout;

    private void Awake()
    {
        skillText.text = skillInfo.skillName;
    }
    public void CheckInteractable()
    {
        if (skillInfo.cost <= playerLoadout.GetSkillPoints() && playerLoadout.GetSkillTier(skillInfo.path) + 1 >= skillInfo.tier && !toggle.isOn)
        {
            if (skillInfo.isWeaponSkill)
            {
                if (playerLoadout.numGunUpgrades != playerLoadout.maxGunUpgrades)
                {
                    toggle.interactable = true;               
                }
                else
                    toggle.interactable = false;
            }
            else
                toggle.interactable = true;
        }
        else
            toggle.interactable = false;
    }

    public void SkillSelected()
    {
        if (!toggle.isOn)
            return;
        playerLoadout.SpendSkillPoints(skillInfo.cost);
        if (playerLoadout.GetSkillTier(skillInfo.path) < skillInfo.tier)
            playerLoadout.SetSkillTier(skillInfo.path, skillInfo.tier);
        if (skillInfo.isWeaponSkill)
            playerLoadout.numGunUpgrades++;
        toggle.interactable = false;
        playerLoadout.UpdateSkillTree();
    }
}
