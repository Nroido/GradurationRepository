using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "GameData/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public ElementType element;

    public int maxHP;
    public int attack;
    public int defense;
    public int defenseBase;
    
    public List<SkillData> skills; // 通常攻撃・アビリティ・奥義をまとめて持つ

    public List<CardData> ownedCards;
}