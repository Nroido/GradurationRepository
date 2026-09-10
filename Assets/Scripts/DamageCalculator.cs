using UnityEngine;

public static class DamageCalculator
{
    public static int CalculateFinalDamage(CharacterRuntimeState attacker, CharacterRuntimeState defender, SkillData skill)
    {
        float defenseBase = defender.baseData.defenseBase;
        float defenseRate = defenseBase / (defenseBase + defender.currentDefense);

        float elementMultiplier = ElementAffinity.GetMultiplier(
            attacker.baseData.element,
            defender.baseData.element
        );

        float randomFactor = Random.Range(0.95f, 1.05f);

        float damage = attacker.currentAttack * skill.power * defenseRate * elementMultiplier * randomFactor;

        return Mathf.Max(1, Mathf.RoundToInt(damage));
    }
}
