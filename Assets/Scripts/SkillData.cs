using UnityEngine;

public enum SkillType
{
    NormalAttack,   // 通常攻撃
    Ability,        // アビリティ
    Ultimate        // 奥義
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "GameData/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public SkillType skillType;
    public ElementType element;
    public float power;          // 威力(ダメージ計算のbasePowerに使う)
    public float cooldown;     // アビリティ用。通常攻撃・奥義は0でもOK
    [TextArea] public string description;
}
