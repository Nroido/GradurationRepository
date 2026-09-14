using UnityEngine;

public enum CardCategory
{
    NormalAttack,   // 通常攻撃
    CharacterSkill, // キャラ専用スキルカード
    Ultimate,       // 必殺技カード
    Generic         // 汎用カード
}


[CreateAssetMenu(fileName = "NewCard", menuName = "GameData/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardCategory category;
    public int cost;
    public SkillData linkedSkill; // ダメージ計算等に使うスキル情報(通常攻撃・スキル・必殺技用)
    [TextArea] public string description;
}
