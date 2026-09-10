using System.Collections.Generic;
using UnityEngine;

public class CharacterRuntimeState
{
    public CharacterData baseData; 

    public int currentHP;
    public int level;
    public int exp;

    // レベルアップで変動する実効値(baseDataの値 + 成長分)
    public int currentAttack;
    public int currentDefense;

    public CharacterRuntimeState(CharacterData data)
    {
        baseData = data;
        level = 1;
        exp = 0;
        currentHP = data.maxHP;
        currentAttack = data.attack;
        currentDefense = data.defense;
    }

    public Dictionary<SkillData, int> skillCooldowns = new Dictionary<SkillData, int>();

    public bool IsSkillReady(SkillData skill)
    {
        return !skillCooldowns.ContainsKey(skill) || skillCooldowns[skill] <= 0;
    }

    public void UseSkill(SkillData skill)
    {
        if (skill.cooldown > 0)
        {
            skillCooldowns[skill] = Mathf.RoundToInt(skill.cooldown);
        }
    }

    public void TickCooldowns()
    {
        var keys = new List<SkillData>(skillCooldowns.Keys);
        foreach (var key in keys)
        {
            if (skillCooldowns[key] > 0) skillCooldowns[key]--;
        }
    }
}