using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "FPS/New Skill")]
public class SkillInfo : ScriptableObject
{
    public string skillName;
    public int cost;
    public int tier;
    public string path; //"tank","support","mobility", "utility"
}
